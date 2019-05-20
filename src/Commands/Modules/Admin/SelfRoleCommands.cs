using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;
using Gommon;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("SelfRoleAdd", "SrA", "SrAdd")]
        [Description("Adds a role to the list of self roles for this guild.")]
        [Remarks("Usage: |prefix|selfroleadd {role}")]
        [RequireGuildAdmin]
        public async Task SelfRoleAddAsync([Remainder] SocketRole role)
        {
            var data = Db.GetData(Context.Guild);
            config.SelfRoles.Add(role.Name);
            Db.UpdateData(config);
            await Context.CreateEmbed($"Successfully added **{role.Name}** to the Self Roles list for this guild.")
                .SendToAsync(Context.Channel);
        }

        [Command("SelfRoleRemove", "SrR", "SrRem")]
        [Description("Removes a role from the list of self roles for this guild.")]
        [Remarks("Usage: |prefix|selfrole remove {role}")]
        [RequireGuildAdmin]
        public async Task SelfRoleRemoveAsync([Remainder] SocketRole role)
        {
            var data = Db.GetData(Context.Guild);

            if (config.SelfRoles.ContainsIgnoreCase(role.Name))
            {
                config.SelfRoles.Remove(role.Name);
                await Context.CreateEmbed($"Removed **{role.Name}** from the Self Roles list for this guild.")
                    .SendToAsync(Context.Channel);
                Db.UpdateData(config);
            }
            else
            {
                await Context.CreateEmbed($"The Self Roles list for this guild doesn't contain **{role.Name}**.")
                    .SendToAsync(Context.Channel);
            }
        }

        [Command("SelfRoleClear", "SrC", "SrClear", "SelfroleC")]
        [Description("Clears the self role list for this guild.")]
        [Remarks("Usage: |prefix|selfroleclear")]
        [RequireGuildAdmin]
        public async Task SelfRoleClearAsync()
        {
            var data = Db.GetData(Context.Guild);
            config.SelfRoles.Clear();
            Db.UpdateData(config);
            await Context.CreateEmbed("Successfully cleared all Self Roles for this guild.")
                .SendToAsync(Context.Channel);
        }
    }
}