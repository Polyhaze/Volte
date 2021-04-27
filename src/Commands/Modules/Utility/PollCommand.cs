using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Poll")]
        [Description("Create a poll.")]
        public Task<ActionResult> PollAsync(
            [Remainder,
             Description(
                 "The content of the poll. Format is `question;option1[;option2;option3;option4;option5]`. You do not need to provide the brackets.")]
            string poll) 
            => PollInfo.TryParse(poll, out var pollInfo)
                ? Ok(pollInfo)
                : BadRequest(pollInfo.Validation.InvalidationReason);
        }
}