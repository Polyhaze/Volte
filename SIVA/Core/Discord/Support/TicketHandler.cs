using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using SIVA.Core.Files.Objects;

namespace SIVA.Core.Discord.Support {
    public class TicketHandler {
        public static async Task OnTicketCreation(SocketCommandContext ctx, RestTextChannel channel, Server config) {
            var embed = new EmbedBuilder()
                .WithTitle("What do you need help with?")
                .WithColor(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB)
                .WithDescription(
                    $"```{ctx.Message.Content}```\n\nIf you're done with this ticket, run `{config.CommandPrefix}close` or react to this message with ☑.")
                .WithAuthor(ctx.User)
                .WithThumbnailUrl(ctx.User.GetAvatarUrl())
                .WithFooter(DateTime.UtcNow.ToLongDateString())
                .WithCurrentTimestamp();

            var msg = await channel.SendMessageAsync("", false, embed.Build());
            await msg.AddReactionAsync(new Emoji("☑"));
            await msg.PinAsync();
        }

        public static async Task DeleteTicket(ISocketMessageChannel channel) {
            await Task.Delay(45000);
            await ((SocketTextChannel) channel).DeleteAsync();
        }
    }
}