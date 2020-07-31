using System.Text;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace BrackeysBot.Commands
{
    public partial class UserModule : BrackeysBotModule
    { 
        [Command("giverole"), Alias("addrole", "role")]
        [Summary("Grants you the desired role, if available.")]
        [Remarks("giverole <role>")]
        [RequireContext(ContextType.Guild)]
        public async Task GiveRoleAsync(
            [Summary("The desired role."), Remainder] IRole role)
        {
            IGuildUser user = Context.User as IGuildUser;

            EmbedBuilder builder = GetDefaultBuilder();

            if (UserService.IsUserRole(role))
            {
                if (!user.RoleIds.Contains(role.Id))
                {
                    await user.AddRoleAsync(role);

                    builder.WithDescription($"Successfully added the role **{role.Name}**!")
                        .WithColor(Color.Green);
                }
                else
                {
                    builder.WithDescription("You already have that role!")
                        .WithColor(Color.Red);
                }
            }
            else
            {
                builder.WithDescription("You don't have permission to get this role.")
                    .WithColor(Color.Red);
            }

            await builder.Build().SendToChannel(Context.Channel);
        }

        [Command("removerole"), Alias("takerole")]
        [Summary("Removes a specified role from you.")]
        [Remarks("removerole <role>")]
        [RequireContext(ContextType.Guild)]
        public async Task RemoveRoleAsync(
            [Summary("The role to remove."), Remainder] IRole role)
        {
            IGuildUser user = Context.User as IGuildUser;

            EmbedBuilder builder = GetDefaultBuilder();

            if (UserService.IsUserRole(role))
            {
                if (user.RoleIds.Contains(role.Id))
                {
                    await user.RemoveRoleAsync(role);

                    builder.WithDescription($"Successfully removed the role **{role.Name}**!")
                        .WithColor(Color.Green);
                }
                else
                {
                    builder.WithDescription("You don't have the specified role.")
                        .WithColor(Color.Red);
                }
            }
            else
            {
                builder.WithDescription("You don't have permission to remove this role.")
                    .WithColor(Color.Red);
            }

            await builder.Build().SendToChannel(Context.Channel);
        }

        [Command("roles"), Alias("listroles")]
        [Summary("Lists the available user roles.")]
        [RequireContext(ContextType.Guild)]
        public async Task ListRolesAsync()
        {
            await GetDefaultBuilder()
                .WithTitle("User Roles")
                .WithDescription(new StringBuilder()
                    .AppendLine()
                    .AppendJoin('\n', UserService.GetUserRoles(Context.Guild)?.Select(t => "• " + t.Name))
                    .ToString())
                .Build()
                .SendToChannel(Context.Channel);
        }
    }
}