using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.BotOwner
{
    public partial class BotOwnerModule
    {
        [Command("ForceBin")]
        [Description("Forcefully creates a debug dump on bin.greemdev.net with debug information used for Support, using the configuration from the target guild.")]
        [Remarks("Usage: |prefix|forcebin")]
        [RequireBotOwner]
        public async Task ForceBinAsync([Remainder] SocketGuild guild)
        {
            await Context.CreateEmbed(
                    "Take this URL to [Volte's Support Discord](https://greemdev.net/Discord) for support with this bot." +
                    "\n" +
                    "\n" +
                    $"https://bin.greemdev.net/{BinService.Execute(Db.GetData(guild))}")
                .SendToAsync(Context.Channel);
        }
    }
}
