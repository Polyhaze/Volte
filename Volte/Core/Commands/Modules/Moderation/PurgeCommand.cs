using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Moderation {
    public partial class ModerationModule : VolteModule {

        [Command("Purge"), Alias("clear", "clean")]
        [Summary("Purges the last x messages.")]
        [Remarks("Usage: $purge {count}")]
        [RequireGuildModerator]
        public async Task Purge(int count) {
            await ((ITextChannel) Context.Channel).DeleteMessagesAsync(
                await Context.Channel.GetMessagesAsync(count +1).FlattenAsync()); 
            //+1 to include the command invocation message, and actually delete the last x messages instead of x - 1.
            
            var msg = await Reply(Context.Channel, CreateEmbed(Context, 
                $"Successfully deleted **{count}** {(count != 1 ? "messages" : "message")}"));
            await Task.Delay(5000);
            await msg.DeleteAsync();
        }
        
    }
}