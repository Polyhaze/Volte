using System.Threading.Tasks;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Information.Patreon {
    public class VerifyCheckCommand : SIVACommand {
        [Command("VerifyCheck"), Alias("Vc")]
        public async Task VerifyCheck() {
            var verifiedMessage = ServerConfig.Get(Context.Guild).VerifiedGuild
                ? "You're verified! Enjoy your donator perks."
                : "You're not verfied. If you've donated and you want your server verified, join the bot's " +
                  "[support server](https://greem.xyz/discord) and ask. If you haven\'t donated, " +
                  $"feel free to donate on <{SIVA.GetPatreonLink()}>!";
            await Context.Channel.SendMessageAsync("", false, CreateEmbed(Context, verifiedMessage));
        }
    }
}