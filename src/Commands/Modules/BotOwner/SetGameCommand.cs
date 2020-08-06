using Discord;
using Gommon;
using Qmmands;
using System.Threading.Tasks;
using Volte.Commands.Results;
using Volte.Core.Attributes;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("SetGame")]
        [Description("Sets the bot's game (presence).")]
        [Remarks("setgame {String}")]
        [RequireBotOwner]
        public Task<ActionResult> SetGameAsync([Remainder] string game)
        {
            var activity = Context.Client.Activity;
            return Context.Client.Activity.Type is ActivityType.Streaming
                ? Ok($"Set the bot's game to **{game}**.", _ => Context.Client.SetGameAsync(game, activity.Cast<StreamingGame>().Url, activity.Type))
                : Ok($"Set the bot's game to **{game}**.", _ => Context.Client.SetGameAsync(game));
        }
    }
}
