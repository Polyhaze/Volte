using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using SIVA.Core.Files.Objects;

namespace SIVA.Core.Discord.Support
{
    public class TicketHandler
    {
        public static async Task OnTicketCreation(SocketCommandContext ctx, RestTextChannel channel, Server config)
        {
            var embed = new EmbedBuilder()
                .WithTitle("What do you need help with?")
                .WithDescription(
                    $"```{ctx.Message.Content}```\n\nIf you're done with this ticket, run `{config.CommandPrefix}close` or react to this message with ☑.")
                .WithAuthor(ctx.User)
                .WithThumbnailUrl(ctx.User.GetAvatarUrl())
                .WithFooter(DateTime.UtcNow.ToString())
                .WithCurrentTimestamp();

            var msg = await channel.SendMessageAsync("", false, embed.Build());
            await msg.AddReactionAsync(new Emoji("☑"));
        }
    }
}