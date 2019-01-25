using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Core.Files.Readers;

namespace Volte.Core.Modules.General {
    public partial class GeneralModule : VolteModule {
        [Command("Info")]
        public async Task Info() {
            var embed = new EmbedBuilder()
                .AddField("Version", "V2.0.0-RELEASE")
                .AddField("Author", "<@168548441939509248>")
                .AddField("Language", "C# - Discord.Net 2.0.1")
                .AddField("Server", "https://greemdev.net/discord")
                .AddField("Server Count", (await Context.Client.GetGuildsAsync()).Count)
                .AddField("Invite Me", "https://greemdev.net/bot")
                .AddField("Ping", VolteBot.Client.Latency)
                .AddField("Client ID", VolteBot.Client.CurrentUser.Id)
                .WithThumbnailUrl("https://pbs.twimg.com/media/Cx0i4LOVQAIyLRU.png")
                .WithAuthor(Context.User)
                .WithColor(Config.GetSuccessColor())
                .Build();

            await Reply(Context.Channel, embed);
        }
    }
}