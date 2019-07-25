using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule
    {
        [Command("ForceBin")]
        [Description(
            "Forcefully creates a debug dump on bin.greemdev.net with debug information used for Support, using the configuration from the target guild.")]
        [Remarks("Usage: |prefix|forcebin")]
        [RequireBotOwner]
        public Task<ActionResult> ForceBinAsync([Remainder] SocketGuild guild)
            => Ok(
                "Take this URL to [Volte's Support Discord](https://greemdev.net/Discord) for support with this bot." +
                "\n" +
                "\n" +
                $"https://bin.greemdev.net/{BinService.Execute(Db.GetData(guild))}");
    }
}