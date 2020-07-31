using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BrackeysBot.Commands
{
    public partial class ModerationModule : BrackeysBotModule
    {
        [Command("clear")]
        [Summary("Deletes a specified amount of messages from the channel.")]
        [Remarks("clear <count>")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task ClearMessagesAsync(
            [Summary("The amount of messages to clear")] int count)
        {
            int maxHistory = Context.Configuration.ClearMessageMaxHistory;
            
            count = count + 1 > maxHistory ? maxHistory : count + 1;

            var messages = await Context.Channel.GetMessagesAsync(count).FlattenAsync();
            var validMessages = messages.Where(m => (DateTimeOffset.Now - m.CreatedAt).Days < 14);
            await (Context.Channel as ITextChannel).DeleteMessagesAsync(validMessages);

            await ModerationLog.CreateEntry(ModerationLogEntry.New
                .WithDefaultsFromContext(Context)
                .WithActionType(ModerationActionType.ClearMessages)
                .WithChannel(Context.Channel as ITextChannel));
        }

        [Command("clear")]
        [Summary("Attempt to delete the specified amount of messages by user from the channel.")]
        [Remarks("clear <user> <count> [history = 100]")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task ClearMessagesAsync(
            [Summary("The user to clear messages of")] SocketGuildUser user,
            [Summary("The amount of messages to clear")] int count,
            [Summary("The history length to delete from")] int history = 100) 
        {
            int maxHistory = Context.Configuration.ClearMessageMaxHistory;
            if (history > maxHistory) 
            {
                history = maxHistory;
            }

            var aMessages = await Context.Channel.GetMessagesAsync(history).FlattenAsync();
            var fMessages = aMessages.Where(m => m.Author.Id == user.Id)
                                    .Where(m => (DateTimeOffset.Now - m.CreatedAt).Days < 14);

            if (fMessages.Count() > 0) 
            {
                var messages = fMessages.Take(count);
                await (Context.Channel as ITextChannel).DeleteMessagesAsync(messages);

                await ModerationLog.CreateEntry(ModerationLogEntry.New
                    .WithDefaultsFromContext(Context)
                    .WithReason($"Deleted {messages.Count()} message(s) of {user.Mention}")
                    .WithActionType(ModerationActionType.ClearMessages)
                    .WithChannel(Context.Channel as ITextChannel));
            }
        }
    }
}
