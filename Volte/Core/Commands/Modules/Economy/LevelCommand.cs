using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Data;
using Volte.Core.Extensions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Economy {
    public partial class EconomyModule : VolteModule {
        [Command("Level")]
        [Summary("Shows the level for the given user, or for yourself if no user is given.")]
        [Remarks("Usage: |prefix|level [@user]")]
        public async Task Level(SocketGuildUser user = null) {
            if (user is null) user = (SocketGuildUser) Context.User;
            await Context.CreateEmbed($"User {user.Mention} is level **{Db.GetUser(Context.User).Level}**.")
                .SendTo(Context.Channel);
        }
    }
}