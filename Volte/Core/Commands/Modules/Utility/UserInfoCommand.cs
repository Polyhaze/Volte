using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Data;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("UserInfo"), Alias("UI")]
        [Summary("Shows info for the mentioned user or yourself if none is provided.")]
        [Remarks("Usage: $userinfo [@user]")]
        public async Task UserInfo(SocketGuildUser user = null) {
            var actualUser = user ?? (SocketGuildUser)Context.User;

            await Context.CreateEmbed(string.Empty)
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
                .SendTo(Context.Channel);
        }
    }
}