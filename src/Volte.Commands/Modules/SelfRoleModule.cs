using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    [Group("SelfRole", "Sr")]
    public sealed class SelfRoleModule : VolteModule {
        [Command("List", "L")]
        [Description("Gets a list of self roles available for this guild.")]
        [Remarks("selfrole list")]
        public Task<ActionResult> SelfRoleListAsync()
        {
            if (Context.GuildData.Extras.SelfRoles.IsEmpty())
                return BadRequest("No roles available to self-assign in this guild.");
            else
            {
                var pages = Context.GuildData.Extras.SelfRoles
                    .Select(x => Context.Guild.Roles.FirstOrDefault(r => r.Value.Name.EqualsIgnoreCase(x)).Value)
                    .Where(r => r is not null);

                return None(async () =>
                {
                    await Context.Interactivity.SendPaginatedMessageAsync(Context.Channel, Context.Member,
                        pages.Select(x => x.Name).GetPages(10));
                }, false);
            }
        }

        [Command("Add", "A")]
        [Description("Adds a role to the list of self roles for this guild.")]
        [Remarks("selfrole add {Role}")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleAddAsync([Remainder, RequiredArgument] DiscordRole role)
        {
            var target = Context.GuildData.Extras.SelfRoles.FirstOrDefault(x => x.EqualsIgnoreCase(role.Name));
            if (target is null)
            {
                return BadRequest(
                    $"A role with the name **{role.Name}** is already in the Self Roles list for this guild!");
            }

            ModifyData(data =>
            {
                data.Extras.SelfRoles.Add(role.Name);
                return data;
            });
            return Ok($"Successfully added **{role.Name}** to the Self Roles list for this guild.");
        }

        [Command("Remove", "R", "Rem")]
        [Description("Removes a role from the list of self roles for this guild.")]
        [Remarks("selfrole remove {Role}")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleRemoveAsync([Remainder, RequiredArgument] DiscordRole role)
        {
            if (Context.GuildData.Extras.SelfRoles.ContainsIgnoreCase(role.Name))
            {
                ModifyData(data =>
                {
                    data.Extras.SelfRoles.RemoveAt(Context.GuildData.Extras.SelfRoles.IndexOf(role.Name));
                    return data;
                });
                return Ok($"Removed **{role.Name}** from the Self Roles list for this guild.");
            }

            return BadRequest($"The Self Roles list for this guild doesn't contain **{role.Name}**.");
        }

        [Command("Clear", "C", "Cl")]
        [Description("Clears the self role list for this guild.")]
        [Remarks("selfrole clear")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleClearAsync()
        {
            ModifyData(data =>
            {
                data.Extras.SelfRoles.Clear();
                return data;
            });
            return Ok("Successfully cleared all Self Roles for this guild.");
        }
    }
}