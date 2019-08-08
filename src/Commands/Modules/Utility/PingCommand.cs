using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Discord;
 
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Ping")]
        [Description("Show the Gateway latency to Discord.")]
        [Remarks("Usage: |prefix|ping")]
        public Task<ActionResult> PingAsync()
        {
            return Ok(async () =>
            {
                var e = Context.CreateEmbedBuilder("Pinging...");
                var sw = new Stopwatch();
                sw.Start();
                var msg = await e.SendToAsync(Context.Channel);
                sw.Stop();
                await msg.ModifyAsync(x =>
                {
                    e.WithDescription(new StringBuilder()
                        .AppendLine($"{EmojiService.Clap} **Ping**: {sw.ElapsedMilliseconds}ms")
                        .AppendLine($"{EmojiService.OkHand} **API**: {Context.Client.Latency}ms")
                        .ToString());
                    x.Embed = e.Build();
                });
            });
        }
    }
}