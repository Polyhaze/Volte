using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Core.Data;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.General {
    public partial class GeneralModule : VolteModule {
        [Command("Info")]
        [Summary("Provides basic information about this instance of Volte.")]
        [Remarks("Usage: |prefix|info")]
        public async Task Info() {
            var embed = new EmbedBuilder()
                .AddField("Version", "V2.0.0-RELEASE")
                .AddField("Author", "<@168548441939509248>")
                .AddField("Language", "C# - Discord.Net 2.0.1")
                .AddField("Server", "https://greemdev.net/discord")
                .AddField("Server Count", (await Context.Client.GetGuildsAsync()).Count)
                .AddField("Invite Me", 
                    $"https://discordapp.com/oauth2/authorize?client_id={VolteBot.Client.CurrentUser.Id}&scope=bot&permissions=8")
                .AddField("Ping", VolteBot.Client.Latency)
                .AddField("Client ID", VolteBot.Client.CurrentUser.Id)
                .WithThumbnailUrl("https://pbs.twimg.com/media/Cx0i4LOVQAIyLRU.png")
                .WithAuthor(Context.User)
                .WithColor(Config.GetSuccessColor());

            await embed.SendTo(Context.Channel);
        }
    }
}