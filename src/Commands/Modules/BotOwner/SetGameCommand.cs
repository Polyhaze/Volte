using Discord;
using Gommon;
using Qmmands;
using System.Threading.Tasks;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("SetGame")]
        [Description("Sets the bot's game (presence).")]
        public Task<ActionResult> SetGameAsync([Remainder, Description("The name of the status to set.")] string game)
        {
            var activity = Context.Client.Activity;
            return activity.Type is ActivityType.Streaming
                ? Ok($"Set the bot's game to {Format.Bold(game)}.", _ => Context.Client.SetGameAsync(game, activity.Cast<StreamingGame>().Url, activity.Type))
                : Ok($"Set the bot's game to {Format.Bold(game)}.", _ => Context.Client.SetGameAsync(game));
        }
    }
}
