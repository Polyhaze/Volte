using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        [Command("SetGame")]
        [Description("Sets the bot's game (presence).")]
        [Remarks("Usage: |prefix|setgame {game}")]
        [RequireBotOwner]
        public async Task SetGameAsync([Remainder] string game)
        {
            await Context.Client.UpdateStatusAsync(new DiscordActivity(game, ActivityType.Playing));
            await Context.CreateEmbed($"Set the bot's game to **{game}**.").SendToAsync(Context.Channel);
        }
    }
}