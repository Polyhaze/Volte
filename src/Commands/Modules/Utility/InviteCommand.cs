using System.Threading.Tasks;
using Qmmands;
using Volte.Data.Models.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Invite")]
        [Description("Get an invite to use Volte in your own guild.")]
        [Remarks("Usage: |prefix|invite")]
        public Task<VolteCommandResult> InviteAsync()
        {
            return Ok("Do you like Volte? If you do, that's awesome! If not then I'm sorry :( \n\n" +
                      "[Website](https://volte.greemdev.net)\n" +
                      $"[Invite Me]({Context.Client.GetInviteUrl(true)})\n" +
                      "[Support Server Invite](https://greemdev.net/discord)\n\n" +
                      "And again, thanks for using me!");
        }
    }
}