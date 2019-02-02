using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Modules.Economy {
    public partial class EconomyModule : VolteModule {
        [Command("Level")]
        [Summary("Shows the level for the given user, or for yourself if no user is given.")]
        [Remarks("Usage: |prefix|level [@user]")]
        public async Task Level(SocketGuildUser user = null) {
            if (user is null) user = (SocketGuildUser) Context.User;

            var userData = Db.GetUser(Context.User);
            await Reply(Context.Channel,
                CreateEmbed(Context, $"User {user.Mention} is level **{userData.Level}**."));
        }
    }
}