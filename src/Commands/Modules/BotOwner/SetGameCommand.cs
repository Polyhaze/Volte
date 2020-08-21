using Gommon;
using Qmmands;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("SetGame")]
        [Description("Sets the bot's game (presence).")]
        [Remarks("setgame {String}")]
        public async Task<ActionResult> SetGameAsync([Remainder]string game)
        {
            await Context.Client.UpdateStatusAsync(new DiscordActivity(game, ActivityType.Playing));
            return Ok($"Set my game to {game}!");
        }
    }
}
