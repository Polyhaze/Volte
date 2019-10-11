using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Poll")]
        [Description("Create a poll.")]
        [Remarks("Usage: |prefix|poll question;option1;option2;...")]
        public Task<ActionResult> PollAsync([Remainder] string pollText)
        {
            var choices = pollText.Split(';');

            return Ok(Context.CreateEmbedBuilder()
                    .WithTitle(choices[0])
                    .WithThumbnailUrl("http://survation.com/wp-content/uploads/2016/09/polleverywherelogo.png")
                    .WithDescription(PollHelpers.GetPollBody(choices, EmojiService)),
                async msg =>
                {
                    _ = await Context.Message.TryDeleteAsync();
                    await PollHelpers.AddPollReactionsAsync(choices, msg, EmojiService);
                });
        }
    }
}