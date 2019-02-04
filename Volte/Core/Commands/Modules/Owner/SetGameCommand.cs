using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Discord;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("SetGame")]
        [Summary("Sets the bot's game (presence).")]
        [Remarks("Usage: $setgame {game}")]
        [RequireBotOwner]
        public async Task SetGame([Remainder] string game) {
            await VolteBot.Client.SetGameAsync(game);
            await Context.Channel.SendMessageAsync(string.Empty, false,
                CreateEmbed(Context, $"Set the bot's game to **{game}**."));
        }
    }
}