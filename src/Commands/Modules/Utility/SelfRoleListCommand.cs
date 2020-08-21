using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class SelfRoleModule
    {
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
                return Ok(new PaginatedMessageBuilder(Context)
                    .WithPages(pages.Select(x => x.Name))
                    .SplitPages(10)
                    .Build());
            }
        }
    }
}