using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Humanizer;

namespace BrackeysBot.Commands
{
    public partial class UtilityModule : BrackeysBotModule
    {
        [Command("userinfo")]
        [Summary("Displays information about a specified user.")]
        [Remarks("userinfo <user>")]
        public async Task ShowUserInfoAsync(IGuildUser user)
        {
            Color userColor = Color.LightGrey;
            if (user.RoleIds.Count > 1)
                userColor = user.RoleIds
                    .Select(id => Context.Guild.GetRole(id))
                    .OrderByDescending(role => role.Position)
                    .First().Color;

            int infractionCount = 0;
            int starCount = 0;
            if (Data.UserData.TryGetUser(user.Id, out UserData data))
            {
                infractionCount = data.Infractions.Count;
                starCount = data.Stars;
            }

            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle($"Information about {user}:")
                .WithThumbnailUrl(user.EnsureAvatarUrl())
                .WithColor(userColor)
                .AddField("Username", user.ToString(), true)
                .AddField("ID", user.Id.ToString(), true)
                .AddFieldConditional(!string.IsNullOrEmpty(user.Nickname), "Nickname", user.Nickname, true)
                .AddFieldConditional(user.JoinedAt.HasValue, "Join Date", user.JoinedAt?.ToShortDateString(), true)
                .AddField("User Created", user.CreatedAt.ToShortDateString(), true)
                .AddFieldConditional(starCount > 0, "Endorsements", $"{starCount} :star:", true)
                .AddFieldConditional(infractionCount > 0, "Infractions", infractionCount.ToString(), true)
                .AddField("Permission Level", user.GetPermissionLevel(Context).Humanize(), true);

            await builder.Build().SendToChannel(Context.Channel);
        }
    }
}