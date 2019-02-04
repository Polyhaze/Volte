using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("SetName")]
        [Summary("Sets the bot's username.")]
        [Remarks("$setname {name}")]
        [RequireBotOwner]
        public async Task SetName([Remainder] string name) {
            await Context.Client.CurrentUser.ModifyAsync(u => u.Username = name);
            await Reply(Context.Channel, $"Set my name to **{name}**.");
        }
    }
}