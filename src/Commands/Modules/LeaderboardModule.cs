using Discord.Commands;

using BrackeysBot.Services;

namespace BrackeysBot.Commands
{
    [Summary("Provides a leaderboard with points to compete in the server.")]
    [ModuleColor(0x4ecc5c)]
    public partial class LeaderboardModule
    {
        public LeaderboardService Leaderboard { get;set; }
    }
}