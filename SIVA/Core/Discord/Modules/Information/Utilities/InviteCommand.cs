using System.Threading.Tasks;
using Discord.Commands;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Information.Utilities {
    public class InviteCommand : SIVACommand {
        [Command("Invite")]
        public async Task Invite() {
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context,
                    "Do you like SIVA? If you do, that\'s awesome! If not then I\'m sorry :( \n\n" +
                    $"[Invite Me](https://discordapp.com/oauth2/authorize?client_id={SIVA.Client.CurrentUser.Id}&scope=bot&permissions=8)\n" +
                    "[Support Server Invite](https://greemdev.net/discord)\n\n" +
                    "And again, Thanks for using me!"));
        }
    }
}