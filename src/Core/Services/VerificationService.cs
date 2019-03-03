using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Core.Discord;

namespace Volte.Core.Services
{
    public sealed class VerificationService : IService
    {
        private readonly DatabaseService _db = VolteBot.GetRequiredService<DatabaseService>();
        private readonly EmojiService _emoji = VolteBot.GetRequiredService<EmojiService>();

        public async Task CheckReactionAsync(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (channel is IDMChannel) return;
            var c = channel as IGuildChannel;
            var config = _db.GetConfig(c?.Guild);
            if (!config.VerificationOptions.Enabled || !reaction.User.IsSpecified) return;
            var u = reaction.User.Value as IGuildUser;
            if (u.IsBot) return;
            if (message.Id.Equals(config.VerificationOptions.MessageId))
            {
                if (reaction.Emote.Name.Equals(_emoji.BALLOT_BOX_WITH_CHECK))
                {
                    var role = c.Guild.GetRole(config.VerificationOptions.RoleId);
                    if (role is null) return;
                    await u.AddRoleAsync(role);

                }
            }
        }

    }
}
