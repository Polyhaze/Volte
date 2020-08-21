using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("SetName")]
        [Description("Sets the bot's username.")]
        [Remarks("setname {String}")]
        public Task<ActionResult> SetNameAsync([Remainder]string name) 
            => Ok($"Set my username to **{name}**.", _ => Context.Client.UpdateCurrentUserAsync(name));
    }
}