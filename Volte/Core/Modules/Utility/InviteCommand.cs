using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Helpers;

namespace Volte.Core.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("Invite")]
        [Summary("Get an invite to use Volte in your own guild.")]
        public async Task Invite() {
            await Reply(Context.Channel,
                CreateEmbed(Context,
                    "Do you like Volte? If you do, that's awesome! If not then I'm sorry :( \n\n" +
                    $"[Invite Me](https://discordapp.com/oauth2/authorize?client_id={VolteBot.Client.CurrentUser.Id}&scope=bot&permissions=8)\n" +
                    "[Support Server Invite](https://greemdev.net/discord)\n\n" +
                    "And again, thanks for using me!"));
        }
    }
}