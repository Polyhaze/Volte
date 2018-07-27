using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration.Support
{
    public class RoleCommand : SIVACommand
    {
        //not registered as a command, as the support system is broken
        public async Task SupportRole([Remainder]string roleName)
        {                        
            if (!UserUtils.IsAdmin(Context))
            {
                await Context.Message.AddReactionAsync(new Emoji(new RawEmoji().X));
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