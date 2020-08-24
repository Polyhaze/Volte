using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Gommon;
using Volte.Core.Helpers;

namespace Volte.Services
{
    public sealed class StarboardService : VolteEventService
    {
        private readonly DatabaseService _db;
        private readonly DiscordShardedClient _client;

        // Ensures star operations are atomic for a given user on a given message.
        private readonly AsyncDuplicateLock<(ulong message, ulong member)> _starrerLock;

        // Ensures starboard message creations don't happen twice, and edits are atomic.
        private readonly AsyncDuplicateLock<ulong> _messageWriteLock;

        private static readonly DiscordEmoji StarEmoji = DiscordEmoji.FromUnicode(EmojiHelper.Star);

        public StarboardService(DatabaseService databaseService, DiscordShardedClient discordShardedClient,
            AsyncDuplicateLock<(ulong, ulong)> starrerLock, AsyncDuplicateLock<ulong> messageWriteLock)
        {
            _db = databaseService;
            _client = discordShardedClient;
            _starrerLock = starrerLock;
            _messageWriteLock = messageWriteLock;
        }


        public override Task DoAsync(EventArgs args)
        {
            return args switch
            {
                MessageReactionAddEventArgs reactionAdd => HandleReactionAddAsync(reactionAdd),
                MessageReactionRemoveEventArgs reactionRemove => HandleReactionRemoveAsync(reactionRemove),
                MessageReactionsClearEventArgs reactionsClear => HandleReactionsClearAsync(reactionsClear),
                _ => Task.CompletedTask
            };
        }

        private async Task HandleReactionAddAsync(MessageReactionAddEventArgs args)
        {
            if (args.Channel is DiscordDmChannel) return;
            if (args.Emoji.Name != EmojiHelper.Star) return;
            if (args.User.IsCurrent) return;
            
            var data = _db.GetData(args.Guild.Id);
            var starboard = data.Configuration.Starboard;
            
            var starboardChannel = await args.Client.GetChannelAsync(starboard.StarboardChannel);
            if (starboardChannel is null) return;
            if (args.Channel == starboardChannel) return; // TODO Support starring the starboard message

            var messageId = args.Message.Id;
            var starrerId = args.User.Id;
            
            if (data.Extras.StarboardedMessages.TryGetValue(messageId, out var entry))
            {
                using (await _starrerLock.LockAsync((messageId, starrerId)))
                {
                    // Add the star to the database
                    if (entry.StarredUserIds.Add(starrerId))
                    {
                        // Update message star count
                        await UpdateOrPostToStarboardAsync(starboard, args.Message, entry);
                    }
                    else
                    {
                        // Invalid star! Either the starboard post or the actual message already has a reaction by this user.
                        if (starboard.DeleteInvalidStars)
                        {
                            await args.Message.DeleteReactionAsync(StarEmoji, args.User, "Star reaction is invalid: User has already starred!");
                        }
                    }
                }
            }
            else
            {
                if (args.Message.Reactions.FirstOrDefault(e => e.Emoji == StarEmoji)?.Count >= starboard.StarsRequiredToPost)
                {
                    // Create new star message!
                    using (await _starrerLock.LockAsync((messageId, starrerId)))
                    {
                        var newEntry = data.Extras.StarboardedMessages.AddOrUpdate(
                            messageId,
                            id => new StarboardEntry
                            {
                                StarredUserIds = {starrerId},
                                MessageId = messageId
                            },
                            (id, existingEntry) =>
                            {
                                existingEntry.StarredUserIds.Add(starrerId);
                                return existingEntry;
                            }
                        );
                        await UpdateOrPostToStarboardAsync(starboard, args.Message, newEntry);
                    }
                }
            }
        }

        private async Task HandleReactionRemoveAsync(MessageReactionRemoveEventArgs args)
        {
            
        }

        private async Task HandleReactionsClearAsync(MessageReactionsClearEventArgs args)
        {
            
        }

        private async Task UpdateOrPostToStarboardAsync(StarboardOptions starboard, DiscordMessage message, StarboardEntry entry)
        {
            var starboardChannel = message.Channel.Guild.GetChannel(starboard.StarboardChannel);
            if (starboardChannel is null)
            {
                return;
            }

            using (await _messageWriteLock.LockAsync(message.Id))
            {
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

            var e = new DiscordEmbedBuilder()
                .WithSuccessColor()
                .WithDescription(message.Content)
                .WithAuthor(message.Author)
                .AddField("Original Message", message.JumpLink);

            var result = await starboardChannel.SendMessageAsync($"{EmojiHelper.Star} {starCount}", embed: e.Build());
            await result.CreateReactionAsync(StarEmoji);
            return result;
        }
    }
}
