using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        [Command("SetName")]
        [Description("Sets the bot's username.")]
        [Remarks("Usage: |prefix|setname {name}")]
        [RequireBotOwner]
        public async Task SetNameAsync([Remainder] string name)
        {
            await Context.Client.CurrentUser.ModifyAsync(u => u.Username = name);
            await Context.CreateEmbed($"Set my name to **{name}**.").SendToAsync(Context.Channel);
        }
    }
}