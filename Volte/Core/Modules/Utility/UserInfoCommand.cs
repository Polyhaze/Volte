using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Files.Readers;

namespace Volte.Core.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("UserInfo"), Alias("UI")]
        [Summary("Shows info for the mentioned user or yourself if none is provided.")]
        [Remarks("Usage: $userinfo [@user]")]
        public async Task UserInfo(SocketGuildUser user = null) {
            var actualUser = user ?? (SocketGuildUser)Context.User;
            

            await Context.Channel.SendMessageAsync("", false, CreateEmbed(Context, "")
                .ToEmbedBuilder()
                .WithAuthor(Context.User)
                .WithThumbnailUrl(actualUser.GetAvatarUrl())
                .WithTitle("User Info")

                .AddField("User ID", actualUser.Id)
                .AddField("Game", actualUser.Activity.Name ?? "Nothing")
                .AddField("Created At",
                    $"Month: {actualUser.CreatedAt.Month}\nDay: {actualUser.CreatedAt.Day}\nYear: {actualUser.CreatedAt.Year}")
                .AddField("Status", actualUser.Status)
                .AddField("Is Bot", actualUser.IsBot)
            .Build());


        }
    }
}