using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Owner {
    public class SetGameCommand : SIVACommand {
        [Command("SetGame")]
        public async Task SetGame([Remainder] string game) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            await SIVA.Client.SetGameAsync(game);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Set the bot's game to **{game}**."));
        }
    }
}