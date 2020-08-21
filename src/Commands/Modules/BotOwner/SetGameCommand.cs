using Discord;
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
        public Task<ActionResult> SetGameAsync([Remainder]string game)
        {
            // TODO: DSharpPlus does not cache activities. The only way to implement this is manually caching.
            var activity = Context.Client.Activity;
            return Context.Client.Activity.Type is ActivityType.Streaming
                ? Ok($"Set the bot's game to **{game}**.", _ => Context.Client.UpdateStatusAsync(game, activity.Cast<StreamingGame>().Url, activity.Type))
                : Ok($"Set the bot's game to **{game}**.", _ => Context.Client.UpdateStatusAsync(game));
        }
    }
}
