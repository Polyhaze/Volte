using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("UserInfo", "UI")]
        [Description("Shows info for the mentioned user or yourself if none is provided.")]
        [Remarks("Usage: $userinfo [@user]")]
        public async Task UserInfoAsync(DiscordMember user = null)
        {
            var actualUser = user ?? Context.User;

            await Context.CreateEmbedBuilder()
                .WithAuthor(Context.User)
                .WithThumbnailUrl(actualUser.AvatarUrl)
                .WithTitle("User Info")
                .AddField("User ID", actualUser.Id.ToString())
                .AddField("Game", actualUser.Presence.Activity.Name ?? "Nothing")
                .AddField("Created At",
                    $"Month: {actualUser.CreationTimestamp.Month}\nDay: {actualUser.CreationTimestamp.Day}\nYear: {actualUser.CreationTimestamp.Year}")
                .AddField("Status", actualUser.Presence.Status.ToString())
                .AddField("Is Bot", actualUser.IsBot.ToString())
                .SendToAsync(Context.Channel);
        }
    }
}