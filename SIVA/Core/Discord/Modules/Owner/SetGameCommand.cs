using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Owner
{
    public class SetGameCommand : SIVACommand
    {
        [Command("SetGame")]
        public async Task SetGame([Remainder]string game)
        {
            if (!Utils.IsBotOwner(Context.User))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }
            await DiscordLogin.Client.SetGameAsync(game);
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, $"Set the bot's game to **{game}**."));
        }
    }
}