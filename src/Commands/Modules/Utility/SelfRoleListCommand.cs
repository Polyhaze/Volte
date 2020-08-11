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
                var pages = Context.GuildData.Extras.SelfRoles;

                return Ok(PaginatedMessageBuilder.New
                    .WithDefaults(Context)
                    .WithPages(pages)
                    .SplitPages(10)
                    .Build());
            }
        }
    }
}