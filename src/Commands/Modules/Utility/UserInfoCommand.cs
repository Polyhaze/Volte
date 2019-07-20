using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("UserInfo", "UI")]
        [Description("Shows info for the mentioned user or yourself if none is provided.")]
        [Remarks("Usage: |prefix|userinfo [user]")]
        public Task<VolteCommandResult> UserInfoAsync(SocketGuildUser user = null)
        {
            var target = user ?? Context.User;


            return Ok(Context.CreateEmbedBuilder()
                .WithAuthor(Context.User)
                .WithThumbnailUrl(target.GetAvatarUrl())
                .WithTitle("User Info")
                .AddField("User ID", target.Id, true)
                .AddField("Game", target.Activity?.Name ?? "Nothing", true)
                .AddField("Status", target.Status, true)
                .AddField("Is Bot", target.IsBot, true)
                .AddField("Account Created",
                    $"{target.CreatedAt.FormatDate()}, {target.CreatedAt.FormatFullTime()}")
                .AddField("Joined This Guild",
                    $"{(target.JoinedAt.HasValue ? target.JoinedAt.Value.FormatDate() : "\u200B")}, " +
                    $"{(target.JoinedAt.HasValue ? target.JoinedAt.Value.FormatFullTime() : "\u200B")}"));
        }
    }
}