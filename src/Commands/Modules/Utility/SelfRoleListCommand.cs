using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Interactive;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("SelfRoleList", "Srl")]
        [Description("Gets a list of self roles available for this guild.")]
        [Remarks("selfrolelist")]
        public Task<ActionResult> SelfRoleListAsync()
        {
            if (Context.GuildData.Extras.SelfRoles.IsEmpty())
                return BadRequest("No roles available to self-assign in this guild.");
            else
            {
                var roles = Context.GuildData.Extras.SelfRoles;
                var pages = new List<string>();

                do
                {
                    pages.Add(roles.Take(10).Select(x => $"**{x}**").Join(""));
                    roles.RemoveRange(0, roles.Count < 10 ? roles.Count : 10);
                } while (!roles.IsEmpty());

                return Ok(new PaginatedMessage
                {
                    Author = Context.User,
                    Pages = pages
                });
            }
        }
    }
}