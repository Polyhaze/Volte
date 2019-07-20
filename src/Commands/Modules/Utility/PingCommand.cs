using System.Diagnostics;
using System.Threading.Tasks;
using Qmmands;
using Volte.Data.Models.Results;
using Volte.Extensions;

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
                        $"{EmojiService.CLAP} **Ping**: {sw.ElapsedMilliseconds}ms \n" +
                        $"{EmojiService.OK_HAND} **API**: {Context.Client.Latency}ms");
                    x.Embed = e.Build();
                });
            });
        }
    }
}