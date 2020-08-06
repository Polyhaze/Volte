using System.Text;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Invite")]
        [Description("Get an invite to use Volte in your own guild.")]
        [Remarks("invite")]
        public Task<ActionResult> InviteAsync()
            => Ok(new StringBuilder()
                .AppendLine(
                    "Do you like Volte? If you do, that's awesome! If not then I'm sorry (please tell me what you don't like [here](https://forms.gle/CJ9XtKmKf2Q2mQwb7)!) :( ")
                .AppendLine()
                .AppendLine("[Website](https://greemdev.net/Volte)")
                .AppendLine($"[Invite Me]({Context.Client.GetInviteUrl()})")
                .AppendLine("[Support Guild Invite](https://discord.gg/H8bcFr2)")
                .AppendLine()
                .AppendLine("And again, thanks for using me!")
                .ToString());
    }
}