using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Helpers;

namespace Volte.Core.Modules.Information.Utilities {
    public class InviteCommand : VolteCommand {
        [Command("Invite")]
        public async Task Invite() {
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context,
                    "Do you like SIVA? If you do, that\'s awesome! If not then I\'m sorry :( \n\n" +
                    $"[Invite Me](https://discordapp.com/oauth2/authorize?client_id={VolteBot.Client.CurrentUser.Id}&scope=bot&permissions=8)\n" +
                    "[Support Server Invite](https://greemdev.net/discord)\n\n" +
                    "And again, Thanks for using me!"));
        }
    }
}