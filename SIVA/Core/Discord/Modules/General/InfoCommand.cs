using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord.Modules.General {
    public class InfoCommand : SIVACommand {
        [Command("Info")]
        public async Task Info() {
            var config = ServerConfig.Get(Context.Guild);
            var embed = new EmbedBuilder()
                .AddField("Version", "V2.0.0-RELEASE")
                .AddField("Author", "<@168548441939509248>")
                .AddField("Language", "C# - Discord.Net 2.0.1")
                .AddField("Server", "https://greemdev.net/discord")
                .AddField("Server Count", (await Context.Client.GetGuildsAsync()).Count)
                .AddField("Invite Me", "https://greemdev.net/bot")
                .AddField("Ping", SIVA.Client.Latency)
                .AddField("Client ID", SIVA.Client.CurrentUser.Id)
                .WithThumbnailUrl("https://pbs.twimg.com/media/Cx0i4LOVQAIyLRU.png")
                .WithAuthor(Context.User)
                .WithColor(new Color(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB))
                .Build();

            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}