using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.General {
    public partial class GeneralModule : VolteModule {
        [Command("Help"), Alias("H")]
        public async Task Help() {
            var embed = CreateEmbed(Context,
                "Use [this link](https://discordapp.com/oauth2/authorize?client_id=320942091049893888&scope=bot&permissions=8) to invite the bot into your server.\n" +
                "Join our [Support Server](https://discord.gg/prR9Yjq)!\n" +
                "Developer: **Greem#1337**\n" +
                "Full command [documentation](https://greemdev.atlassian.net/wiki/spaces/SIVA/overview)\n" +
                "**Commands do NOT work in Private Messages.**");

            await React(Context.SMessage, RawEmoji.BALLOT_BOX_WITH_CHECK);
            await Reply(await Context.User.GetOrCreateDMChannelAsync(), embed);
        }
    }
}