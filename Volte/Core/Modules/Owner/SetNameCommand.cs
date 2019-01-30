using System.Threading.Tasks;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("SetName")]
        [Summary("Sets the bot's username.")]
        [Remarks("$setname {name}")]
        public async Task SetName([Remainder] string name) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }
            await Context.Client.CurrentUser.ModifyAsync(u => u.Username = name);
            await Reply(Context.Channel, $"Set my name to **{name}**.");
        }
    }
}