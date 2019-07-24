using System.Diagnostics;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Ping")]
        [Description("Show the Gateway latency to Discord.")]
        [Remarks("Usage: |prefix|ping")]
        public Task<VolteCommandResult> PingAsync()
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
                    e.WithDescription(
                        $"{EmojiService.Clap} **Ping**: {sw.ElapsedMilliseconds}ms \n" +
                        $"{EmojiService.OkHand} **API**: {Context.Client.Latency}ms");
                    x.Embed = e.Build();
                });
            });
        }
    }
}