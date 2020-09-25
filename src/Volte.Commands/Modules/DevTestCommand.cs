using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    [DoNotAdd, Hidden]
    public sealed class DevTestModule : VolteModule
    {
        [Command("DevTest")]
        [Description("Command for developmental testing. This command in its entirety will be commented out for Releases.")]
        [Remarks("devtest")]
        [Hidden]
        [RequireBotOwner]
        public Task<ActionResult> DevTestAsync()
        {
            return None();
        }
    }
}
