namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Ping")]
    [Description("Show the Gateway and REST latency to Discord.")]
    public Task<ActionResult> PingAsync()
        => Ok(async () =>
        {
            var e = Context.CreateEmbedBuilder("Pinging...");
            var sw = new Stopwatch();
            sw.Start();
            var msg = await e.ReplyToAsync(Context.Message);
            sw.Stop();
            await msg.ModifyAsync(x =>
            {
                e.WithDescription(new StringBuilder()
                    .AppendLine($"{Emojis.Clap} **Gateway**: {Context.Client.Latency} milliseconds")
                    .AppendLine($"{Emojis.OkHand} **REST**: {sw.Elapsed.Humanize(3)}"));
                x.Embed = e.Build();
            });
        }, false);
}