using System.Threading.Tasks;
using Discord;
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
            await VolteBot.Client.SetGameAsync(game);
            await Context.CreateEmbed($"Set the bot's game to **{game}**.").SendToAsync(Context.Channel);
        }
    }
}