using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace BrackeysBot.Commands
{
    public partial class LeaderboardModule : BrackeysBotModule
    {
        [Command("leaderboard"), Alias("top", "lb")]
        public async Task DisplayLeaderboardAsync()
        {
            var leaderboard = Leaderboard.GetLeaderboard();

            await GetDefaultBuilder()
                .WithTitle("Leaderboard")
                .WithFields(Leaderboard.GetLeaderboard()
                    .Select((l, i) => new EmbedFieldBuilder()
                        .WithName((i + 1).ToString().Envelop("**"))
                        .WithValue($"{l.User.Mention} · {l.Points} points")
                        .WithIsInline(true)))
                .Build()
                .SendToChannel(Context.Channel);
        }
    }
}
