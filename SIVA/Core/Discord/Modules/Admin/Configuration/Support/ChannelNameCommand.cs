using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration.Support
{
    public class ChannelNameCommand : SIVACommand
    {
        [Command("SupportChannelName"), Alias("Scn")]
        public async Task SupportChannelName([Remainder]string cName)
        {
            if (!UserUtils.IsAdmin(Context))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.SupportChannelName = cName;
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, $"Set the Support Channel name to **{cName}** for this server."));
        }
    }
}