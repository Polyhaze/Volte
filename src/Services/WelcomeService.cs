using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core;
using Volte.Data.Models.EventArgs;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("Welcome", "The main Service that handles welcome/leaving functionality..")]
    public sealed class WelcomeService
    {
        public static WelcomeService Instance = VolteBot.GetRequiredService<WelcomeService>();

        private readonly DatabaseService _db;

        public WelcomeService(DatabaseService databaseService)
        {
            _db = databaseService;
        }

        internal async Task JoinAsync(UserJoinedEventArgs args)
        {
            if (args.Data.Configuration.Welcome.WelcomeMessage.IsNullOrEmpty())
                return; //we don't want to send an empty join message
            var welcomeMessage = args.Data.Configuration.Welcome.WelcomeMessage
                .Replace("{ServerName}", args.Guild.Name)
                .Replace("{UserName}", args.User.Username)
                .Replace("{UserMention}", args.User.Mention)
                .Replace("{OwnerMention}", (await args.Guild.GetOwnerAsync()).Mention)
                .Replace("{UserTag}", args.User.Discriminator)
                .Replace("{MemberCount}", (await args.Guild.GetUsersAsync()).Count.ToString())
                .Replace("{UserString}", args.User.ToString());
            var c = await args.Guild.GetTextChannelAsync(args.Data.Configuration.Welcome.WelcomeChannel);

            if (!(c is null))
            {
                var embed = new EmbedBuilder()
                    .WithColor(args.Data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(welcomeMessage)
                    .WithThumbnailUrl(args.User.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await embed.SendToAsync(c);
            }
        }

        internal async Task LeaveAsync(UserLeftEventArgs args)
        {
            if (args.Data.Configuration.Welcome.LeavingMessage.IsNullOrEmpty()) return;
            var leavingMessage = args.Data.Configuration.Welcome.LeavingMessage
                .Replace("{ServerName}", args.Guild.Name)
                .Replace("{UserName}", args.User.Username)
                .Replace("{UserMention}", args.User.Mention)
                .Replace("{OwnerMention}", (await args.Guild.GetOwnerAsync()).Mention)
                .Replace("{UserTag}", args.User.Discriminator)
                .Replace("{MemberCount}", (await args.Guild.GetUsersAsync()).Count.ToString())
                .Replace("{UserString}", args.User.ToString());
            var c = await args.Guild.GetTextChannelAsync(args.Data.Configuration.Welcome.WelcomeChannel);
            if (!(c is null))
            {
                var embed = new EmbedBuilder()
                    .WithColor(
                        args.Data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(leavingMessage)
                    .WithThumbnailUrl(args.User.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await embed.SendToAsync(c);
            }
        }
    }
}