using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord.Support {
    public static class ReactionHandler {
        public static async Task CheckMessageForEmoji(Cacheable<IUserMessage, ulong> userCache,
            ISocketMessageChannel channel, SocketReaction reaction) {
            var config = ServerConfig.Get(((SocketTextChannel) channel).Guild);
            if (reaction.Emote.Equals(new Emoji("☑"))
                && Regex.IsMatch(channel.Name, "^" + config.SupportChannelName + "-[0-9]{18}$")
                && reaction.UserId != SIVA.GetInstance.CurrentUser.Id) {
                await channel.SendMessageAsync("", false, new EmbedBuilder()
                    .WithAuthor(reaction.User.Value)
                    .WithDescription("Closing ticket in 45 seconds...")
                    .WithColor(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB).Build());
                await TicketHandler.DeleteTicket(channel);
            }
        }
    }
}