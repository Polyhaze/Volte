using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace BrackeysBot.Commands
{
    public partial class LeaderboardModule : BrackeysBotModule
    {
        [Command("points"), Alias("p")]
        [Summary("Displays the points of a user, or your own points.")]
        [Remarks("points [user]")]
        public async Task DisplayUserPointsAsync(
            [Summary("The optional user to display the points for.")] IGuildUser user = null)
        {
            if (user == null)
            {
                await DisplaySelfPointsAsync();
            }
            else
            {
                await GetDefaultBuilder()
                    .WithDescription($"{user.Mention} has **{Leaderboard.GetUserPoints(user)}** points.")
                    .WithFooter(Leaderboard.FormatRank(user))
                    .Build()
                    .SendToChannel(Context.Channel);
            }
        }

        [Command("points"), Alias("p")]
        [HideFromHelp]
        public async Task DisplaySelfPointsAsync()
        {
            await GetDefaultBuilder()
                .WithDescription($"You have **{Leaderboard.GetUserPoints(Context.User)}** points.")
                .WithFooter(Leaderboard.FormatRank(Context.User))
                .Build()
                .SendToChannel(Context.Channel);
        }

        [Command("setpoints"), Alias("sp")]
        [Summary("Sets the points of a user.")]
        [Remarks("setpoints <user> <amount>")]
        [RequireModerator]
        public async Task SetPointsAsync(
            [Summary("The user to set the points for.")] IGuildUser user,
            [Summary("The amount of points to set the user's points to.")] int amount)
        {
            int clamped = Math.Clamp(amount, 0, int.MaxValue);
            Leaderboard.SetUserPoints(user, clamped);

            await GetDefaultBuilder()
                .WithDescription($"{user.Mention} now has **{clamped}** points!")
                .Build()
                .SendToChannel(Context.Channel);
        }

        [Command("addpoints"), Alias("ap")]
        [Summary("Adds a specified amount to a user's points.")]
        [Remarks("addpoints <user> <amount>")]
        [RequireModerator]
        public async Task AddPointsAsync(
            [Summary("The user to add points to.")] IGuildUser user,
            [Summary("The amount of points to add.")] int amount)
        {
            int newAmount = Leaderboard.GetUserPoints(user) + amount;
            await SetPointsAsync(user, newAmount);
        }

        [Command("removepoints"), Alias("rp")]
        [Summary("Removes a specified amount to a user's points.")]
        [Remarks("removepoints <user> <amount>")]
        [RequireModerator]
        public async Task RemovePointsAsync(
            [Summary("The user to remove points from.")] IGuildUser user,
            [Summary("The amount of points to remove.")] int amount)
            => await AddPointsAsync(user, -amount);

        [Command("resetpoints")]
        [Summary("Resets the points of a user.")]
        [Remarks("resetpoints <user>")]
        [RequireModerator]
        public async Task ResetPointsAsync(
            [Summary("The user to reset the points for.")] IGuildUser user)
            => await SetPointsAsync(user, 0);
    }
}
