using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("SelfRoleAdd", "SrAdd")]
        [Description("Adds a role to the list of self roles for this guild.")]
        [Remarks("Usage: |prefix|selfroleadd {roleName}")]
        [RequireGuildAdmin]
        public async Task SelfRoleAddAsync(string roleName)
        {
            if (Context.Guild.Roles.FirstOrDefault(x => x.Name.EqualsIgnoreCase(roleName)) is null)
            {
                await Context.CreateEmbed("That role doesn't exist in this guild.").SendTo(Context.Channel);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.SelfRoles.Add(roleName);
            Db.UpdateConfig(config);
            await Context.CreateEmbed($"Successfully added **{roleName}** to the Self Roles for this guild.")
                .SendTo(Context.Channel);
        }

        [Command("SelfRoleRemove", "SrRem")]
        [Description("Removes a role from the list of self roles for this guild.")]
        [Remarks("Usage: |prefix|selfrole remove {roleName}")]
        [RequireGuildAdmin]
        public async Task SelfRoleRemAsync(string roleName)
        {
            var config = Db.GetConfig(Context.Guild);

            if (config.SelfRoles.Any(x => x.EqualsIgnoreCase(roleName)))
            {
                config.SelfRoles.Remove(roleName);
                await Context.CreateEmbed($"Removed **{roleName}** from the Self Roles list on this guild.")
                    .SendTo(Context.Channel);
                Db.UpdateConfig(config);
            }
            else
            {
                await Context.CreateEmbed($"The Self Roles list for this guild doesn't contain **{roleName}**.")
                    .SendTo(Context.Channel);
            }
        }

        [Command("SelfRoleClear", "SrC", "SrClear", "SelfroleC")]
        [Description("Clears the self role list for this guild.")]
        [Remarks("Usage: |prefix|selfroleclear")]
        [RequireGuildAdmin]
        public async Task SelfRoleClearAsync()
        {
            var config = Db.GetConfig(Context.Guild);
            config.SelfRoles.Clear();
            Db.UpdateConfig(config);
            await Context.CreateEmbed("Successfully cleared all Self Roles for this server.").SendTo(Context.Channel);
        }
    }
}