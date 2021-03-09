using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
using Gommon;
using Humanizer;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Ping")]
        [Description("Show the Gateway latency to Discord.")]
        [Remarks("ping")]
        public Task<ActionResult> PingAsync()
            => None(async () =>
            {
                var e = Context.CreateEmbedBuilder("Pinging...");
                var sw = new Stopwatch();
                sw.Start();
                var msg = await e.ReplyToAsync(Context.Message);
                sw.Stop();
                await msg.ModifyAsync(x =>
                {
                    e.WithDescription(new StringBuilder()
                        .AppendLine($"{EmojiService.Clap} **Gateway**: {Context.Client.Latency} milliseconds")
                        .AppendLine($"{EmojiService.OkHand} **REST**: {sw.Elapsed.Humanize(3)}")
                        .ToString());
                    x.Embed = e.Build();
                });
            }, false);
    }
}