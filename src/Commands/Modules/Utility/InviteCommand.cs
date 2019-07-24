using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
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
            return Ok(
                "Do you like Volte? If you do, that's awesome! If not then I'm sorry (please tell me what you don't like [here](https://forms.gle/CJ9XtKmKf2Q2mQwb7)!) :( \n\n" +
                "[Website](https://greemdev.net/Volte)\n" +
                $"[Invite Me]({Context.Client.GetInviteUrl()})\n" +
                "[Support Server Invite](https://greemdev.net/discord)\n\n" +
                "And again, thanks for using me!");
        }
    }
}