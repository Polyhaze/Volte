using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Volte.Services
{
    [Service("Verification", "The main Service that handles user verification.")]
    public sealed class VerificationService
    {
        private readonly DatabaseService _db;
        private readonly EmojiService _emoji;

        public VerificationService(DatabaseService databaseService,
                                   EmojiService emojiService)
        {
            _db = databaseService;
            _emoji = emojiService;
        }

        public async Task CheckReactionAsync(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (channel is IDMChannel) return;
            if (!(reaction.User.Value is IGuildUser u) || !(channel is IGuildChannel c)) return;
            var config = _db.GetConfig(c.Guild);
            if (!config.VerificationOptions.Enabled || !reaction.User.IsSpecified) return;
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
