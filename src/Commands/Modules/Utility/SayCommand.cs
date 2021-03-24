using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Core;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Say")]
        [Description("Bot repeats what you tell it to.")]
        public Task<ActionResult> SayAsync([Remainder, Description("What to say.")] string msg) 
            => None(async () =>
            {
                await Context.CreateEmbed(msg).SendToAsync(Context.Channel);
                _ = await Context.Message.TryDeleteAsync();
            });

        [Command("SilentSay", "SSay")]
        [Description("Runs the say command normally, but doesn't show the author in the message. Useful for announcements.")]
        public Task<ActionResult> SilentSayAsync([Remainder, Description("What to say.")] string msg) 
            => None(async () =>
            {
                await new EmbedBuilder()
                    .WithColor(Config.SuccessColor)
                    .WithDescription(msg)
                    .SendToAsync(Context.Channel);
                _ = await Context.Message.TryDeleteAsync();
            });


        public Task<ActionResult> SayPlainAsync([Remainder, Description("What to say.")] string msg)
            => None(async () =>
            {
                await Context.Channel.SendMessageAsync(msg, allowedMentions: AllowedMentions.None);
            });
    }
}