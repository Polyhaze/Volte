using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

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

        public async Task CheckReactionAsync(MessageReactionAddEventArgs args)
        {
            if (args.Channel is DiscordDmChannel) return;
            if (!(args.User is DiscordMember u) || !(args.Channel is DiscordChannel c)) return;
            var config = _db.GetConfig(c.Guild);
            if (!config.VerificationOptions.Enabled) return;
            if (u.IsBot) return;
            if (args.Message.Id.Equals(config.VerificationOptions.MessageId))
            {
                if (args.Emoji.Name.Equals(_emoji.BALLOT_BOX_WITH_CHECK))
                {
                    var role = c.Guild.GetRole(config.VerificationOptions.RoleId);
                    if (role is null) return;
                    await u.GrantRoleAsync(role, "User verified.");

                }
            }
        }

    }
}
