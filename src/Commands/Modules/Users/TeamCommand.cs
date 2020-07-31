using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace BrackeysBot.Commands
{
    public partial class UserModule : BrackeysBotModule
    {
        [Command("team"), Alias("jointeam")]
        [Summary("Joins the specified team, leaving other teams that you are in.")]
        [Remarks("team <name>")]
        [RequireContext(ContextType.Guild)]
        public async Task JoinTeamAsync(
            [Summary("The name of the team to join.")] string name)
        {
            string teamName = $"Team {name}";

            IGuildUser user = Context.User as IGuildUser;
            IRole currentTeam = UserService.GetTeam(user, Context.Guild);
            IRole targetTeam = UserService.GetTeamByName(teamName, Context.Guild);

            EmbedBuilder builder = GetDefaultBuilder();

            if (targetTeam == null)
            {
                await builder.WithColor(Color.Red)
                    .WithDescription("That team does not exist. Type `[]teams` for a list of teams!")
                    .Build()
                    .SendToChannel(Context.Channel);
            }
            else
            {
                if (currentTeam != targetTeam)
                {
                    if (currentTeam != null)
                        await user.RemoveRoleAsync(currentTeam);

                    await user.AddRoleAsync(targetTeam);

                    builder.WithDescription($"You sucessfully joined **{targetTeam.Name}**.")
                        .WithColor(Color.Green);
                }
                else
                {
                    builder.WithDescription($"You are already in **{targetTeam.Name}**.")
                        .WithColor(Color.Red);
                }

                await builder.Build().SendToChannel(Context.Channel);
            }
        }

        [Command("leaveteam")]
        [Summary("Leaves the team that you are in.")]
        [RequireContext(ContextType.Guild)]
        public async Task LeaveTeamAsync()
        {
            IGuildUser user = Context.User as IGuildUser;
            IRole currentTeam = UserService.GetTeam(user, Context.Guild);

            EmbedBuilder builder = GetDefaultBuilder();

            if (currentTeam != null)
            {
                await user.RemoveRoleAsync(currentTeam);

                builder.WithDescription($"You successfully left **{currentTeam.Name}**.")
                    .WithColor(Color.Green);
            }
            else
            {
                builder.WithDescription("You dont currently have a team.")
                    .WithColor(Color.Red);
            }

            await builder.Build().SendToChannel(Context.Channel);
        }

        [Command("teams"), Alias("showteams")]
        [Summary("Displays a list of available teams.")]
        [RequireContext(ContextType.Guild)]
        public async Task ShowTeamsAsync()
        {
            await GetDefaultBuilder()
                .WithTitle("Teams")
                .WithDescription(new StringBuilder("")
                    .AppendLine()
                    .AppendJoin('\n', UserService.GetTeams(Context.Guild)?.Select(t => "• " + t.Name))
                    .ToString())
                .Build()
                .SendToChannel(Context.Channel);
        }
    }
}
