using Qmmands;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("SetStream")]
        [Description("Sets the bot's stream via Twitch username and Stream name, respectively.")]
        [Remarks("setstream {String} {String}")]
        public async Task<ActionResult> SetStreamAsync(string stream, [Remainder] string game)
        {
            await Context.Client.UpdateStatusAsync(new DiscordActivity
            {
                Name = game,
                ActivityType = ActivityType.Streaming,
                StreamUrl = $"https://twitch.tv/{stream}"
            });
            return Ok(
                $"Set the bot's game to **{game}**, and the Twitch URL to **[{stream}](https://twitch.tv/{stream})**.");
        }
    }
}
