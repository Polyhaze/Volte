using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core.Data.Models.EventArgs;

namespace Volte.Services
{
    [Service("Welcome", "The main Service that handles welcome/leaving functionality.")]
    public sealed class WelcomeService
    {
        private readonly DatabaseService _db;

        public WelcomeService(DatabaseService databaseService)
        {
            _db = databaseService;
        }

        internal async Task JoinAsync(UserJoinedEventArgs args)
        {
            var data = _db.GetData(args.Guild);
            if (data.Configuration.Welcome.WelcomeMessage.IsNullOrEmpty())
                return; //we don't want to send an empty join message
            var welcomeMessage = data.Configuration.Welcome.WelcomeMessage
                .Replace("{ServerName}", args.Guild.Name)
                .Replace("{UserName}", args.User.Username)
                .Replace("{UserMention}", args.User.Mention)
                .Replace("{OwnerMention}", args.Guild.Owner.Mention)
                .Replace("{UserTag}", args.User.Discriminator)
                .Replace("{MemberCount}", args.Guild.MemberCount.ToString())
                .Replace("{UserString}", args.User.ToString());
            var c = args.Guild.GetTextChannel(data.Configuration.Welcome.WelcomeChannel);

            if (!(c is null))
            {
                var embed = new EmbedBuilder()
                    .WithColor(data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(welcomeMessage)
                    .WithThumbnailUrl(args.User.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await embed.SendToAsync(c);
            }
        }

        internal async Task LeaveAsync(UserLeftEventArgs args)
        {
            var data = _db.GetData(args.Guild);
            if (data.Configuration.Welcome.LeavingMessage.IsNullOrEmpty()) return;
            var leavingMessage = data.Configuration.Welcome.LeavingMessage
                .Replace("{ServerName}", args.Guild.Name)
                .Replace("{UserName}", args.User.Username)
                .Replace("{UserMention}", args.User.Mention)
                .Replace("{OwnerMention}", args.Guild.Owner.Mention)
                .Replace("{UserTag}", args.User.Discriminator)
                .Replace("{MemberCount}", args.Guild.MemberCount.ToString())
                .Replace("{UserString}", args.User.ToString());
            var c = args.Guild.GetTextChannel(data.Configuration.Welcome.WelcomeChannel);
            if (!(c is null))
            {
                var embed = new EmbedBuilder()
                    .WithColor(data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(leavingMessage)
                    .WithThumbnailUrl(args.User.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await embed.SendToAsync(c);
            }
        }
    }
}