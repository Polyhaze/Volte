using System.Threading.Tasks;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("SetName")]
        public async Task SetName([Remainder] string name) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.BALLOT_BOX_WITH_CHECK);
                return;
            }
            await Context.Client.CurrentUser.ModifyAsync(u => u.Username = name);
            await Reply(Context.Channel, $"Set my name to **{name}**.");
        }
    }
}