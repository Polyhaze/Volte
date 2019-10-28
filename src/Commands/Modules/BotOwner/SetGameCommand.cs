using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("SetGame")]
        [Description("Sets the bot's game (presence).")]
        [Remarks("setgame {game}")]
        [RequireBotOwner]
        public Task<ActionResult> SetGameAsync([Remainder] string game) 
            => Ok($"Set the bot's game to **{game}**.", _ => Context.Client.SetGameAsync(game));
    }
}