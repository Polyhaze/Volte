using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Information.Utilities {
    public class FeedbackCommand : VolteCommand {
        [Command("Feedback"), Alias("Fb")]
        public async Task Feedback([Remainder]string feedback) {
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Feedback sent! Message: ```{feedback}```"));

            var config = ServerConfig.Get(Context.Guild);
            var embed = new EmbedBuilder()
                .WithDescription($"```{feedback}```")
                .WithColor(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB)
                .WithTitle($"Feedback from {Context.User.Username}#{Context.User.Discriminator}");
            var channel = VolteBot.Client.GetGuild(405806471578648588).GetTextChannel(415182876326232064);
            await channel.SendMessageAsync("", false, embed.Build());
        }
    }
}