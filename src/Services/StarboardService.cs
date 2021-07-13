using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Gommon;
using Volte.Core.Helpers;
using Volte.Core.Entities;

namespace Volte.Services
{
    public sealed class StarboardService : IVolteService
    {
        private readonly DatabaseService _db;
        private readonly DiscordShardedClient _client;

        // Ensures starboard message creations don't happen twice, and edits are atomic. Also ensures dictionary updates
        // don't happen at the same time.
        private readonly AsyncDuplicateLock<ulong> _starboardReadWriteLock;

        private readonly Emoji _starEmoji = DiscordHelper.Star.ToEmoji();

        public StarboardService(DatabaseService databaseService, DiscordShardedClient discordShardedClient)
        {
            _db = databaseService;
            _client = discordShardedClient;
            _starboardReadWriteLock = new AsyncDuplicateLock<ulong>();
        }

        /// <summary>
        /// Verifies if a given reaction operation is for a valid starboard reaction (star emoji, not DM, not made by
        /// the bot, and a starboard channel exists).
        /// </summary>
        /// <param name="channel">The channel the reaction was sent in</param>
        /// <param name="reaction">The reaction</param>
        /// <param name="starboard">Will be assigned to retrieved starboard information</param>
        /// <param name="starboardChannel">Will be assigned to the <see cref="SocketChannel"/> for the starboard channel</param>
        /// <returns>True if the reaction is valid, false otherwise</returns>
        private bool IsStarReaction(
            IMessageChannel channel, SocketReaction reaction,
            out StarboardOptions starboard, out SocketChannel starboardChannel)
        {
            starboard = default;
            starboardChannel = default;
            
            // Ignore reaction events sent in DMs
            if (!(channel is IGuildChannel guildChannel)) return false;

            // Ignore non-star reactions
            if (reaction.Emote.Name != _starEmoji.Name) return false;
            
            // Ignore reactions from the current user
            if (reaction.UserId == _client.CurrentUser.Id) return false;

            var data = _db.GetData(guildChannel.Guild.Id);
            starboard = data.Configuration.Starboard;

            if (!starboard.Enabled) return false;

            starboardChannel = _client.GetChannel(starboard.StarboardChannel);
            return !(starboardChannel is null);
        }

        public async Task HandleReactionAddAsync(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            var channel = await cachedChannel.GetOrDownloadAsync();
            
            if (!IsStarReaction(channel, reaction, out var starboard, out var starboardChannel))
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
                    else
                    {
                        // Invalid star! Either the starboard post or the actual message already has a reaction by this user.
                        if (starboard.DeleteInvalidStars)
                        {
                            await message.RemoveReactionAsync(_starEmoji, reaction.UserId, new RequestOptions { AuditLogReason = "Star reaction is invalid: User has already starred!" });
                        }
                    }
                }
            }
            else if (channel != starboardChannel) // Can't make a new starboard message for a post in the starboard channel!
            {
                using (await _starboardReadWriteLock.LockAsync(messageId))
                {
                    if (message.Reactions.FirstOrDefault(e => e.Key.Name == _starEmoji.Name).Value.ReactionCount >= starboard.StarsRequiredToPost)
                    {
                        // Create new star message!
                        entry = new StarboardEntry
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

            if (!IsStarReaction(channel, reaction, out var starboard, out _))
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
                        {
                            _db.RemoveStargazers(entry);
                        }
                        else
                        {
                            _db.UpdateStargazers(entry);
                        }

                        await UpdateOrPostToStarboardAsync(starboard, message, entry);
                    }
                }
            }
        }

        public async Task HandleReactionsClearAsync(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
        {
            var messageChannel = await cachedChannel.GetOrDownloadAsync();

            // Ignore reactions cleared in DMs
            if (!(messageChannel is IGuildChannel channel)) return;

            var guildId = channel.Guild.Id;
            var messageId = cachedMessage.Id;

            var data = _db.GetData(guildId);
            var starboard = data.Configuration.Starboard;

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
                        .Select(x => x.Key)
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
                        {
                            _db.RemoveStargazers(entry);
                        }
                        else
                        {
                            _db.UpdateStargazers(entry);
                        }

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
        /// <returns></returns>
        private async Task UpdateOrPostToStarboardAsync(StarboardOptions starboard, IMessage message, StarboardEntry entry)
        {
            var starboardChannel = _client.GetChannel(starboard.StarboardChannel);
            if (!(starboardChannel is SocketTextChannel starboardTextChannel))
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
                    // Ignore, maybe log to console
                    return;
                }

                if (!(starboardMessage is IUserMessage starboardUserMessage)) return;

                if (entry.StarCount >= starboard.StarsRequiredToPost)
                {
                    // Update existing message
                    var targetMessage = $"{_starEmoji} {entry.StarCount}";
                    if (starboardMessage.Content != targetMessage)
                        await starboardUserMessage.ModifyAsync(e => e.Content = targetMessage);
                }
                else
                {
                    // Unstarred below the limit so delete the message if any
                    await starboardMessage.DeleteAsync();
                    entry.StarboardMessageId = 0;
                }
            }
        }

        private async Task<IMessage> PostToStarboardAsync(IMessage message, int starCount)
        {
            var data = await _db.GetDataAsync(message.Channel.Cast<IGuildChannel>().GuildId);
            
            var starboardChannel = _client.GetChannel(data.Configuration.Starboard.StarboardChannel);
            if (!(starboardChannel is SocketTextChannel starboardTextChannel))
                return null;

            // Discord API limitation: Fetch a full message. The message in OnReactionXXX does not contain an Author
            // field unless it is present in DSharpPlus' message cache.
            message = await message.Channel.GetMessageAsync(message.Id);

            var e = new EmbedBuilder()
                .WithSuccessColor()
                .WithDescription(message.Content)
                .WithAuthor(message.Author)
                .AddField("Original Message", message.GetJumpUrl());

            var result = await starboardTextChannel.SendMessageAsync($"{_starEmoji} {starCount}", embed: e.Build());
            await result.AddReactionAsync(_starEmoji);
            return result;
        }
    }
}