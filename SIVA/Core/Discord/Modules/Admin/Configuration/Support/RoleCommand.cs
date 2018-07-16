using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration.Support
{
    public class RoleCommand : SIVACommand
    {
        [Command("SupportRole"), Alias("Sr")]
        public async Task SupportRole([Remainder]string roleName)
        {
            if (!UserUtils.IsAdmin(Context))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.SupportRole = roleName;
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, $"Set the support role for this server to **{roleName}**."));
        }
    }
}