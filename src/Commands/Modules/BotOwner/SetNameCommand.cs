using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("SetName")]
        [Description("Sets the bot's username.")]
        [Remarks("setname {name}")]
        [RequireBotOwner]
        public Task<ActionResult> SetNameAsync([Remainder] string name) 
            => Ok($"Set my username to **{name}**.", _ => Context.Client.CurrentUser.ModifyAsync(u => u.Username = name));
    }
}