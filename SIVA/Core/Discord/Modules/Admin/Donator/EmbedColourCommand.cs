using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Donator {
    public class EmbedColourCommand : SIVACommand {
        [Command("EmbedColour"), Alias("EmbedColor")]
        public async Task EmbedColour(int r, int g, int b) {
            var config = ServerConfig.Get(Context.Guild);
            if (!UserUtils.IsAdmin(Context) || !config.VerifiedGuild) {
                await React(Context.Message, RawEmoji.X);
                return;
            }

            config.EmbedColourR = r;
            config.EmbedColourG = b;
            config.EmbedColourB = b;
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                new EmbedBuilder()
                    .WithDescription($"Set this server's embed colour to `{r}, {g}, {b}.`")
                    .WithAuthor(Context.User)
                    .WithColor(new Color(r, g, b))
                    .Build()
            );
        }
    }
}