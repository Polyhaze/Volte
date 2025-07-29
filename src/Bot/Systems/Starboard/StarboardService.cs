using System.Net;
using Volte.Systems.Database;
using Volte.Systems.Database.Entities;

namespace Volte.Systems.Starboard;

public sealed class StarboardService : VolteService
{
    private readonly DiscordSocketClient _client;
    private readonly DatabaseService _db;

    public StarboardService(DiscordSocketClient client, DatabaseService databaseService)
    {
        _client = client;
        _db = databaseService;

    }
    
    // Ensures starboard message creations don't happen twice, and edits are atomic. Also ensures dictionary updates
    // don't happen at the same time.
    private readonly AsyncDuplicateLock<ulong> _starboardReadWriteLock = new();

    /// <summary>
    /// Verifies if a given reaction operation is for a valid starboard reaction (star emoji, not DM, not made by
    /// the bot, and a starboard channel exists).
    /// </summary>
    /// <param name="channel">The channel the reaction was sent in</param>
    /// <param name="reaction">The reaction</param>
    /// <param name="starboard">Will be assigned to retrieved starboard information</param>
    /// <param name="starboardChannel">Will be assigned to the <see cref="SocketChannel"/> for the starboard channel</param>
    /// <returns>True if the reaction is valid, false otherwise</returns>
    private bool IsCountableStarReaction(
        IMessageChannel channel,
        IUserMessage starredMessage,
        SocketReaction reaction,
        out StarboardOptions starboard, out SocketChannel starboardChannel)
    {
        starboard = default;
        starboardChannel = default;
        
        if (starredMessage is null)
            return false; //message pointed to is a non-user posted message
        
        // Ignore reactions from the user who posted the star
        if (reaction.UserId == starredMessage.Author.Id) return false;
            
        // Ignore reaction events sent in DMs
        if (channel is not IGuildChannel guildChannel) return false;

        // Ignore non-star reactions
        if (reaction.Emote.Name != Emojis.Star.Name) return false;
            
        // Ignore reactions from the current user
        if (reaction.UserId == _client.CurrentUser.Id) return false;
        
        starboard = _db.GetData(guildChannel.Guild.Id).Configuration.Starboard;

        if (!starboard.Enabled) return false;

        starboardChannel = _client.GetChannel(starboard.StarboardChannel);
        return starboardChannel is not null;
    }

    public async Task HandleReactionAddAsync(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
    {
        var channel = await cachedChannel.GetOrDownloadAsync();
        
        var starredMessage = reaction.Message.IsSpecified
            ? reaction.Message.Value
            : (await channel.GetMessageAsync(reaction.MessageId)).Cast<IUserMessage>();
            
        if (!IsCountableStarReaction(channel, starredMessage, reaction, out var starboard, out var starboardChannel) || reaction.User.IsSpecified && reaction.User.Value.IsBot)
            return;

        var guildId = channel.Cast<IGuildChannel>().Guild.Id;
        var messageId = cachedMessage.Id;
        var starrerId = reaction.UserId;

        var message = await cachedMessage.GetOrDownloadAsync();

        if (_db.TryGetStargazers(guildId, messageId, out var entry))
        {
            using (await _starboardReadWriteLock.LockAsync(entry.StarredMessageId))
            {
                // Add the star to the database
                if (entry.Stargazers.TryAdd(starrerId, channel == starboardChannel ? StarTarget.StarboardMessage : StarTarget.OriginalMessage))
                {
                    // Update message star count
                    await UpdateOrPostToStarboardAsync(starboard, message, entry);

                    _db.UpdateStargazers(entry);
                }
                else if (starboard.DeleteInvalidStars)
                    // Invalid star! Either the starboard post or the actual message already has a reaction by this user.
                    await message.RemoveReactionAsync(Emojis.Star, reaction.UserId,
                        DiscordHelper.RequestOptions(x =>
                            x.AuditLogReason = "Star reaction is invalid: User has already starred!"));
            }
        }
        else if (channel != starboardChannel) // Can't make a new starboard message for a post in the starboard channel!
        {
            using (await _starboardReadWriteLock.LockAsync(messageId))
            {
                if (message.Reactions.FirstOrDefault(e => e.Key.Name == Emojis.Star.Name).Value.ReactionCount >= starboard.StarsRequiredToPost)
                {
                    // Create new star message!
                    entry = new()
                    {
                        GuildId = guildId,
                        StarredMessageId = messageId,
                        StarboardMessageId = 0, // is set in UpdateOrPostToStarboardAsync
                        Stargazers =
                        {
                            [starrerId] = StarTarget.OriginalMessage
                        }
                    };

                    await UpdateOrPostToStarboardAsync(starboard, message, entry);
                }

                _db.UpdateStargazers(entry);
            }
        }
    }

    public async Task HandleReactionRemoveAsync(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
    {
        var channel = await cachedChannel.GetOrDownloadAsync();
        
        var starredMessage = reaction.Message.IsSpecified
            ? reaction.Message.Value
            : (await channel.GetMessageAsync(reaction.MessageId)).Cast<IUserMessage>();

        if (!IsCountableStarReaction(channel, starredMessage, reaction, out var starboard, out _))
            return;

        var guildId = channel.Cast<IGuildChannel>().Guild.Id;
        var messageId = cachedMessage.Id;
        var starrerId = reaction.UserId;

        var message = await cachedMessage.GetOrDownloadAsync();

        if (_db.TryGetStargazers(guildId, messageId, out var entry))
        {
            using (await _starboardReadWriteLock.LockAsync(entry.StarredMessageId))
            {
                var removedStarTarget = messageId == entry.StarredMessageId
                    ? StarTarget.OriginalMessage
                    : StarTarget.StarboardMessage;

                // Remove the star from the database
                if (entry.Stargazers.TryGetValue(starrerId, out var starTarget) && starTarget == removedStarTarget && entry.Stargazers.Remove(starrerId))
                {
                    // Update message star count
                    if (entry.StarCount < starboard.StarsRequiredToPost)
                        _db.RemoveStargazers(entry);
                    else
                        _db.UpdateStargazers(entry);
                        

                    await UpdateOrPostToStarboardAsync(starboard, message, entry);
                }
            }
        }
    }

    public async Task HandleReactionsClearAsync(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
    {
        var messageChannel = await cachedChannel.GetOrDownloadAsync();

        // Ignore reactions cleared in DMs
        if (messageChannel is not IGuildChannel channel) return;

        var guildId = channel.Guild.Id;
        var messageId = cachedMessage.Id;
            
        var starboard = (await _db.GetDataAsync(guildId)).Configuration.Starboard;

        if (!starboard.Enabled) return;

        var starboardChannel = _client.GetChannel(starboard.StarboardChannel);
        if (starboardChannel is null) return;

        if (_db.TryGetStargazers(guildId, messageId, out var entry))
        {
            using (await _starboardReadWriteLock.LockAsync(entry.StarredMessageId))
            {
                var clearedStarTarget = messageId == entry.StarredMessageId
                    ? StarTarget.OriginalMessage
                    : StarTarget.StarboardMessage;

                var clearList = entry.Stargazers
                    .Where(x => x.Value == clearedStarTarget)
                    .Select(static x => x.Key)
                    .ToArray();

                // Remove the stars from the database
                if (clearList.Length > 0)
                {
                    foreach (var userId in clearList)
                    {
                        entry.Stargazers.Remove(userId);
                    }

                    // Update message star count
                    if (entry.StarCount < starboard.StarsRequiredToPost)
                        _db.RemoveStargazers(entry);
                    else
                        _db.UpdateStargazers(entry);
                        

                    var message = await cachedMessage.GetOrDownloadAsync();
                    await UpdateOrPostToStarboardAsync(starboard, message, entry);
                }
            }
        }
    }
        
    /// <summary>
    ///     Updates, posts, or deletes a message in the starboard in a guild.
    ///     Calls to this method should be synchronized to _messageWriteLock beforehand!
    /// </summary>
    /// <param name="starboard">The guild's starboard configuration</param>
    /// <param name="message">The message to star (must be from a <see cref="IGuildChannel"/>)</param>
    /// <param name="entry"></param>
    private async Task UpdateOrPostToStarboardAsync(StarboardOptions starboard, IMessage message, StarboardEntry entry)
    {
        var starboardChannel = _client.GetChannel(starboard.StarboardChannel);
        if (starboardChannel is not SocketTextChannel starboardTextChannel)
            return;

        if (entry.StarboardMessageId == 0)
        {
            if (entry.StarCount >= starboard.StarsRequiredToPost)
            {
                // New message just reached star threshold, send it
                var newMessage = await PostToStarboardAsync(message, entry.StarCount);
                entry.StarboardMessageId = newMessage.Id;
            }
        }
        else
        {
            IMessage starboardMessage;
            try
            {
                starboardMessage = await starboardTextChannel.GetMessageAsync(entry.StarboardMessageId);
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) 
            {
                Debug(LogSource.Service, "Could not retrieve original star message in channel.");
                return;
            }

            if (starboardMessage is not IUserMessage starboardUserMessage) return;

            if (entry.StarCount >= starboard.StarsRequiredToPost)
            {
                // Update existing message
                var targetMessage = $"{Emojis.Star} {entry.StarCount}";
                if (starboardMessage.Content != targetMessage)
                    await starboardUserMessage.ModifyAsync(e => e.Content = targetMessage);
            }
            else
            {
                // Star count below the limit so delete the message if any
                await starboardMessage.DeleteAsync();
                entry.StarboardMessageId = 0;
            }
        }
    }

    public EmbedBuilder GetStarboardEmbed(IMessage message)
    {
        var e = new EmbedBuilder()
            .WithSuccessColor()
            .WithTitle(message.CreatedAt.ToDiscordTimestamp(TimestampType.Relative))
            .WithAuthor(message.Author)
            .AddField("Posted", Format.Bold(Format.Url($"#{message.Channel.Name}", message.GetJumpUrl())));

        if (message.Attachments.Any() && !message.Content.IsNullOrEmpty())
            e.WithDescription(message.Content).WithImageUrl(message.Attachments.First().Url);
        if (message.Attachments.None() && !message.Content.IsNullOrEmpty())
            e.WithDescription(message.Content);
        if (message.Attachments.Any() && message.Content.IsNullOrEmpty())
            e.WithImageUrl(message.Attachments.First().Url);

        if (message.Attachments.Count > 1)
            e.WithFooter($"This message has {message.Attachments.Count - 1} more attachments. See original message.");

        return e;
    }

    private async Task<IMessage> PostToStarboardAsync(IMessage message, int starCount)
    {
        var data = await _db.GetDataAsync(message.Channel.Cast<IGuildChannel>().GuildId);
            
        var starboardChannel = _client.GetChannel(data.Configuration.Starboard.StarboardChannel);
        if (starboardChannel is not ITextChannel starboardTextChannel)
            return null;

        // Discord API limitation: Fetch a full message. The message in OnReactionXXX does not contain an Author
        // field unless it is present in Discord.Net's message cache.
        message = await message.Channel.GetMessageAsync(message.Id);

        var result = await starboardTextChannel.SendMessageAsync($"{Emojis.Star} {starCount}", 
            embed: GetStarboardEmbed(message).Build());
        await result.AddReactionAsync(Emojis.Star);
        return result;
    }
}