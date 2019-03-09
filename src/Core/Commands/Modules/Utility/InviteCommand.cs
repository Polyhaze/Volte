using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Invite")]
        [Description("Get an invite to use Volte in your own guild.")]
        [Remarks("Usage: |prefix|invite")]
        public async Task InviteAsync()
        {
            await Context.CreateEmbed("Do you like Volte? If you do, that's awesome! If not then I'm sorry :( \n\n" +
                                      "[Website](https://volte.greemdev.net)\n" +
                                      $"[Invite Me](https://discordapp.com/oauth2/authorize?client_id={VolteBot.Client.CurrentUser.Id}&scope=bot&permissions=8)\n" +
                                      "[Support Server Invite](https://greemdev.net/discord)\n\n" +
                                      "And again, thanks for using me!")
                .SendToAsync(Context.Channel);
        }
    }
}