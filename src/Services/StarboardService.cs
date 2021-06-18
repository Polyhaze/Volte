using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
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

        public async Task HandleReactionAddAsync(Cacheable<IUserMessage, ulong> _message, ISocketMessageChannel _channel, SocketReaction _reaction)
        {
            if (!IsStarReaction(_message, _channel, _reaction, out var guildId, out var messageId, out var starrerId, out var starboard, out var starboardChannel))
                return;

            var message = await _message.GetOrDownloadAsync();

            if (_db.TryGetStargazers(guildId, messageId, out var entry))
            {
                using (await _starboardReadWriteLock.LockAsync(entry.StarredMessageId))
                {
                    // Add the star to the database
                    if (entry.Stargazers.TryAdd(starrerId, _channel == starboardChannel ? StarTarget.StarboardMessage : StarTarget.OriginalMessage))
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
                            await message.RemoveReactionAsync(_starEmoji, _reaction.UserId, new RequestOptions { AuditLogReason = "Star reaction is invalid: User has already starred!" });
                        }
                    }
                }
            }
            else if (_channel != starboardChannel) // Can't make a new starboard message for a post in the starboard channel!
            {
                using (await _starboardReadWriteLock.LockAsync(messageId))
                {
                    if (message.Reactions.FirstOrDefault(e => e.Key.Name == _starEmoji.Name).Value.ReactionCount >= starboard.StarsRequiredToPost)
                    {
                        // Create new star message!
                        entry = new StarboardEntry2
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

        public async Task HandleReactionRemoveAsync(Cacheable<IUserMessage, ulong> _message, ISocketMessageChannel _channel, SocketReaction _reaction)
        {
            if (!IsStarReaction(_message, _channel, _reaction, out var guildId, out var messageId, out var starrerId, out var starboard, out _))
                return;

            var message = await _message.GetOrDownloadAsync();

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
                            
                            await UpdateOrPostToStarboardAsync(starboard, message, entry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Verifies 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="channel"></param>
        /// <param name="reaction"></param>
        /// <param name="guildId"></param>
        /// <param name="messageId"></param>
        /// <param name="starrerId"></param>
        /// <param name="starboard"></param>
        /// <param name="starboardChannel"></param>
        /// <returns></returns>
        private bool IsStarReaction(
            Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction,
            out ulong guildId, out ulong messageId, out ulong starrerId, out StarboardOptions starboard, out SocketChannel starboardChannel)
        {
            guildId = default;
            messageId = default;
            starrerId = default;
            starboard = default;
            starboardChannel = default;
            
            // Ignore reaction events sent in DMs
            if (!(channel is IGuildChannel guildChannel)) return false;

            // Ignore non-star reactions
            if (reaction.Emote.Name != _starEmoji.Name) return false;
            
            // Ignore reactions from the current user
            if (reaction.UserId == _client.CurrentUser.Id) return false;

            guildId = guildChannel.Guild.Id;
            messageId = message.Id;
            starrerId = reaction.UserId;

            var data = _db.GetData(guildId);
            starboard = data.Configuration.Starboard;

            starboardChannel = _client.GetChannel(starboard.StarboardChannel);
            return !(starboardChannel is null);
        }

        public async Task HandleReactionsClearAsync(Cacheable<IUserMessage, ulong> _message, ISocketMessageChannel _channel, SocketReaction _reaction)
        {
            // Ignore reactions cleared in DMs
            if (!(_channel is IGuildChannel channel)) return;

            var guildId = channel.Guild.Id;
            var messageId = _message.Id;

            var data = _db.GetData(guildId);
            var starboard = data.Configuration.Starboard;
            
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

                            var message = await _message.GetOrDownloadAsync();
                            await UpdateOrPostToStarboardAsync(starboard, message, entry);
                        }
                        else
                        {
                            _db.UpdateStargazers(entry);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        ///     Updates or posts a message to the starboard in a guild.
        ///     Calls to this method should be synchronized to _messageWriteLock beforehand!
        /// </summary>
        /// <param name="starboard">The guild's starboard configuration</param>
        /// <param name="message">The message to star</param>
        /// <param name="entry"></param>
        /// <returns></returns>
        private async Task UpdateOrPostToStarboardAsync(StarboardOptions starboard, IMessage message, StarboardEntry2 entry)
        {
            var starboardChannel = message.Channel.Guild.GetChannel(starboard.StarboardChannel);
            if (starboardChannel is null)
            {
                return;
            }

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
                DiscordMessage starboardMessage;
                try
                {
                    starboardMessage = await starboardChannel.GetMessageAsync(entry.StarboardMessageId);
                }
                catch (NotFoundException)
                {
                    // Ignore, maybe log to console
                    return;
                }

                if (entry.StarCount >= starboard.StarsRequiredToPost)
                {
                    // Update existing message
                    var targetMessage = $"{EmojiHelper.Star} {entry.StarCount}";
                    if (starboardMessage.Content != targetMessage)
                    {
                        await starboardMessage.ModifyAsync(targetMessage);
                    }
                }
                else
                {
                    // Unstarred below the limit so delete the message if any
                    await starboardMessage.DeleteAsync();
                    entry.StarboardMessageId = 0;
                }
            }
        }

        private async Task<DiscordMessage> PostToStarboardAsync(DiscordMessage message, int starCount)
        {
            var data = _db.GetData(message.Channel.Guild);
            
            var starboardChannel = message.Channel.Guild.GetChannel(data.Configuration.Starboard.StarboardChannel);
            if (starboardChannel is null)
            {
                return null;
            }

            // Discord API limitation: Fetch a full message. The message in OnReactionXXX does not contain an Author
            // field unless it is present in DSharpPlus' message cache.
            message = await message.Channel.GetMessageAsync(message.Id);

            var e = new DiscordEmbedBuilder()
                .WithSuccessColor()
                .WithDescription(message.Content)
                .WithAuthor(message.Author)
                .AddField("Original Message", message.JumpLink);

            var result = await starboardChannel.SendMessageAsync($"{_starEmoji} {starCount}", embed: e.Build());
            await result.CreateReactionAsync(_starEmoji);
            return result;
        }
    }
}