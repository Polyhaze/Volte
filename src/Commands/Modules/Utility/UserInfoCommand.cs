using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("UserInfo", "UI")]
        [Description("Shows info for the mentioned user or yourself if none is provided.")]
        [Remarks("Usage: |prefix|userinfo [user]")]
        public async Task UserInfoAsync(SocketGuildUser user = null)
        {
            var target = user ?? Context.User;

            await Context.CreateEmbedBuilder()
                .WithAuthor(Context.User)
                .WithThumbnailUrl(target.GetAvatarUrl())
                .WithTitle("User Info")
                .AddField("User ID", target.Id, true)
                .AddField("Game", target.Activity.Name ?? "Nothing", true)
                .AddField("Status", target.Status, true)
                .AddField("Is Bot", target.IsBot, true)
                .AddField("Account Created",
                    $"{target.CreatedAt.FormatDate()}, {target.CreatedAt.FormatFullTime()}")
                .AddField("Joined This Guild", $"{target.JoinedAt.Value.FormatDate()}, {target.JoinedAt.Value.FormatFullTime()}")
                .SendToAsync(Context.Channel);
        }
    }
}