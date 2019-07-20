using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules.BotOwner
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