using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Core.Helpers;

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
        {
            var content = poll.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var pollInfo = PollHelper.GetPollBody(content);
            if (!pollInfo.Validation.IsValid)
                return BadRequest(pollInfo.Validation.InvalidationReason);
            pollInfo.WithPrompt(content.First());

            return Ok(pollInfo);
        }
    }
}