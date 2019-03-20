using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Ping")]
        [Description("Show the Gateway latency to Discord.")]
        [Remarks("Usage: |prefix|ping")]
        public async Task PingAsync()
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
        }
    }
}