using System.Threading.Tasks;
using Discord.Commands;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Information.Patreon {
    public class DonateCommand : SIVACommand {
        [Command("Donate")]
        public async Task Donate() {
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context,
                    $"If you want to, you can donate to my patreon [here]({SIVA.PatreonLink})."));
        }
    }
}