using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Helpers;
using SIVA.Core.Files.Readers;
using System.Linq;
using Discord;

namespace SIVA.Core.Discord.Modules.Moderation
{
    public class KickCommand : SIVACommand
    {
        [Command("Kick")]
        public async Task Kick(SocketGuildUser user, [Remainder]string reason = "Kicked by a Moderator.")
        {
            var config = ServerConfig.Get(Context.Guild);
            if (!Utils.UserHasRole(user, Context.Guild.Roles.FirstOrDefault(r => r.Id == config.ModRole)))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            await user.GetOrCreateDMChannelAsync().GetAwaiter().GetResult().SendMessageAsync("", false,
                Utils.CreateEmbed(Context, $"You were kicked from **{Context.Guild.Name}** for **{reason}**."));

            await user.KickAsync(reason);
            await Context.Channel.SendMessageAsync("", false, Utils.CreateEmbed(Context, 
                $"Successfully kicked {user.Username}#{user.Discriminator} from this server."));
        }
    }
}