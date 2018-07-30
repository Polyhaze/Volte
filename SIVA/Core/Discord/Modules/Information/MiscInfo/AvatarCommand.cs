using System.Threading.Tasks;
using Discord;
using SIVA.Helpers;
using Discord.Commands;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord.Modules.Information.MiscInfo {
    public class AvatarCommand : SIVACommand{
        [Command("Avatar")]
        public async Task Avatar() {
            var config = ServerConfig.Get(Context.Guild);
            var embed = new EmbedBuilder()
                .WithImageUrl(Context.User.GetAvatarUrl())
                .WithAuthor(Context.User)
                .WithColor(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB);
            
            await Context.Channel.SendMessageAsync("", false, embed.Build());

        }
    }
}