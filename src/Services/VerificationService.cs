using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Data.Objects.EventArgs;

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

        public async Task CheckReactionAsync(ReactionAddedEventArgs args)
        {
            if (args.Channel is IDMChannel) return;
            if (!(args.Reaction.User.Value is IGuildUser u) || !(args.Channel is IGuildChannel c)) return;
            var config = _db.GetConfig(c.Guild);
            if (!config.VerificationOptions.Enabled || !args.Reaction.User.IsSpecified) return;
            if (u.IsBot) return;
            if (args.Message.Id.Equals(config.VerificationOptions.MessageId))
            {
                if (args.Reaction.Emote.Name.Equals(_emoji.BALLOT_BOX_WITH_CHECK))
                {
                    var role = c.Guild.GetRole(config.VerificationOptions.RoleId);
                    if (role is null) return;
                    await u.AddRoleAsync(role);
                }
            }
        }
    }
}