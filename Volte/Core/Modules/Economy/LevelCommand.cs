using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Economy {
    public partial class EconomyModule : VolteModule {
        [Command("Level")]
        public async Task Level(SocketGuildUser user = null) {
            if (user == null) user = (SocketGuildUser) Context.User;

            var userData = Db.GetUser(Context.User);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"User {user.Mention} is level **{userData.Level}**."));
        }
    }
}