using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Ping")]
        [Description("Show the Gateway latency to Discord.")]
        [Remarks("Usage: |prefix|ping")]
        public async Task Ping()
        {
            var e = Context.CreateEmbed("Pinging...").ToEmbedBuilder();
            var sw = new Stopwatch();
            sw.Start();
            var msg = await e.SendTo(Context.Channel);
            sw.Stop();
            await msg.ModifyAsync(x =>
            {
                e.WithDescription(
                    $"{EmojiService.CLAP} **Ping**: {sw.ElapsedMilliseconds}ms \n" +
                    $"{EmojiService.OK_HAND} **API**: {VolteBot.Client.Latency}ms");
                x.Embed = e.Build();
            });
        }
    }
}