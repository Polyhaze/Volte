using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.UserRewards
{
    public class LevelCommand : SIVACommand
    {
        [Command("Level")]
        public async Task Level(SocketGuildUser user = null)
        {
            if (user == null) user = (SocketGuildUser)Context.User;

            var userData = Users.Get(Context.User.Id);
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, $"User {user.Mention} is level **{userData.Level}**."));
        }
    }
}