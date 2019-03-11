using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("UserInfo", "UI")]
        [Description("Shows info for the mentioned user or yourself if none is provided.")]
        [Remarks("Usage: $userinfo [@user]")]
        public async Task UserInfoAsync(SocketGuildUser user = null)
        {
            var actualUser = user ?? Context.User;

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
                .SendToAsync(Context.Channel);
        }
    }
}