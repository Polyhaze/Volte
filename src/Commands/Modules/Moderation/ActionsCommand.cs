using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Models;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Actions")]
        [Description("Show all the moderator actions taken against a user.")]
        [Remarks("actions [User]")]
        public async Task<ActionResult> ActionsAsync([Remainder] DiscordMember user = null)
        {
            user ??= Context.Member;
            var allActions = Context.GuildData.GetUserData(user.Id).Actions;
            var l = new List<string>();

            foreach (var action in allActions)
            {
                var mod = await Context.Client.ShardClients.First().Value.GetUserAsync(action.Moderator);
                var reason = action.Reason.IsNullOrEmpty() ? "No reason provided." : action.Reason;
                var str = action.Type switch
                {
                    ModActionType.Ban => $"**Ban**: {action.Time.FormatDate()}, for `{reason}` by **{mod}**.",
                    ModActionType.ClearWarns => $"**Cleared Warnings**: {action.Time.FormatDate()}, by **{mod}**.",
                    ModActionType.IdBan => $"**Id Ban**: {action.Time.FormatDate()}, for `{ reason}` by **{mod}**.",
                    ModActionType.Kick => $"**Kick**: {action.Time.FormatDate()}, for `{reason}` by **{mod}**.",
                    ModActionType.Softban => $"**Softban**: {action.Time.FormatDate()}, for `{reason}` by **{mod}**.",
                    ModActionType.Warn => $"**Warn**: {action.Time.FormatDate()}, for `{reason}` by **{mod}**.",
                    _ => ""
                };

                l.Add(str);
            }

            return Ok(new PaginatedMessageBuilder(Context).WithPages(l).SplitPages(10));
        }
    }
}
