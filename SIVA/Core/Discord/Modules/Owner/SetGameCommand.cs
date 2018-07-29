using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Owner {
    public class SetGameCommand : SIVACommand {
        [Command("SetGame")]
        public async Task SetGame([Remainder] string game) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await Context.Message.AddReactionAsync(new Emoji(RawEmoji.X));
                return;
            }

            await SIVA.GetInstance().SetGameAsync(game);
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, $"Set the bot's game to **{game}**."));
        }
    }
}