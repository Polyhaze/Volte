using System.Threading.Tasks;
using Discord;
using Volte.Helpers;
using Discord.Commands;
using Volte.Core.Files.Readers;

namespace Volte.Core.Modules.Information.MiscInfo {
    public class AvatarCommand : VolteCommand{
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