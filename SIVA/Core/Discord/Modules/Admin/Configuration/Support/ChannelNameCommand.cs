using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration.Support
{
    public class ChannelNameCommand : SIVACommand
    {
        //not registered as a command, as the support system is broken
        public async Task SupportChannelName([Remainder]string cName)
        {
            if (!UserUtils.IsAdmin(Context))
            {
                await Context.Message.AddReactionAsync(new Emoji(RawEmoji.X));
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.SupportChannelName = cName;
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Set the Support Channel name to **{cName}** for this server."));
        }
    }
}