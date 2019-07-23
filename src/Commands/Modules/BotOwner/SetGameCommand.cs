using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("SetGame")]
        [Description("Sets the bot's game (presence).")]
        [Remarks("Usage: |prefix|setgame {game}")]
        [RequireBotOwner]
        public async Task<VolteCommandResult> SetGameAsync([Remainder] string game)
        {
            await Context.Client.SetGameAsync(game);
            return Ok($"Set the bot's game to **{game}**.");
        }
    }
}