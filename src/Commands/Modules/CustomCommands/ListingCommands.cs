using System.Text;
using System.Linq;
using System.Threading.Tasks;

using Discord.Commands;

namespace BrackeysBot.Commands
{
    public partial class CustomCommandsModule : BrackeysBotModule
    {
        [Command("customcommands"), Alias("cclist", "ccl")]
        [Summary("Displays a list of custom commands.")]
        public async Task DisplayCommandsAsync()
        {
            StringBuilder builder = new StringBuilder()
                .AppendLine("Here is a list of useable commands!")
                .AppendLine()
                .AppendJoin('\n', CustomCommands.GetCommands().Select(c => c.Name));

            await GetDefaultBuilder()
                .WithDescription(builder.ToString())
                .Build()
                .SendToChannel(Context.Channel);
        }
    }
}
