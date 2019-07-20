using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules.BotOwner
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("SetName")]
        [Description("Sets the bot's username.")]
        [Remarks("Usage: |prefix|setname {name}")]
        [RequireBotOwner]
        public async Task<VolteCommandResult> SetNameAsync([Remainder] string name)
        {
            await Context.Client.CurrentUser.ModifyAsync(u => u.Username = name);
            return Ok($"Set my username to **{name}**.");
        }
    }
}