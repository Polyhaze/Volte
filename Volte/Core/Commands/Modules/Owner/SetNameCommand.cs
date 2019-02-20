using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        [Command("SetName")]
        [Description("Sets the bot's username.")]
        [Remarks("$setname {name}")]
        [RequireBotOwner]
        public async Task SetNameAsync([Remainder] string name)
        {
            await Context.Client.CurrentUser.ModifyAsync(u => u.Username = name);
            await Context.CreateEmbed($"Set my name to **{name}**.").SendTo(Context.Channel);
        }
    }
}