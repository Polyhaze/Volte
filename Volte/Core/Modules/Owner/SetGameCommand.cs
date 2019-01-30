using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using Volte.Core.Discord;
using Volte.Helpers;

namespace Volte.Core.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("SetGame")]
        [Summary("Sets the bot's game (presence).")]
        [Remarks("Usage: $setgame {game}")]
        public async Task SetGame([Remainder] string game) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            await VolteBot.Client.SetGameAsync(game);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Set the bot's game to **{game}**."));
        }
    }
}