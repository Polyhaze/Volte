using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Economy {
    public class LevelCommand : VolteCommand {
        [Command("Level")]
        public async Task Level(SocketGuildUser user = null) {
            if (user == null) user = (SocketGuildUser) Context.User;

            var userData = Users.Get(Context.User.Id);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"User {user.Mention} is level **{userData.Level}**."));
        }
    }
}