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
                .AddField("Language", "C# - Discord.Net 2.0.0-beta")
                .AddField("Server", "https://greem.xyz/discord")
                .AddField("Server Count", Context.Client.Guilds.Count)
                .AddField("Invite Me", "https://greem.xyz/bot")
                .AddField("Ping", SIVA.GetInstance().Latency)
                .AddField("Client ID", SIVA.GetInstance().CurrentUser.Id)
                .WithThumbnailUrl("https://pbs.twimg.com/media/Cx0i4LOVQAIyLRU.png")
                .WithAuthor(Context.User)
                .WithColor(new Color(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB))
                .Build();

            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}