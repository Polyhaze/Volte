using System.Threading.Tasks;
using Discord;
using Volte.Data.Objects.EventArgs;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("DefaultWelcome",
        "The main Service that handles default welcome/leaving functionality when no WelcomeApiKey is set in the bot config.")]
    public sealed class DefaultWelcomeService
    {
        private readonly DatabaseService _db;

        public DefaultWelcomeService(DatabaseService databaseService)
        {
            _db = databaseService;
        }

        internal async Task JoinAsync(UserJoinedEventArgs args)
        {
            if (args.Config.WelcomeOptions.WelcomeMessage.IsNullOrEmpty())
                return; //we don't want to send an empty join message
            var welcomeMessage = args.Config.WelcomeOptions.WelcomeMessage
                .Replace("{ServerName}", args.Guild.Name)
                .Replace("{UserName}", args.User.Username)
                .Replace("{UserMention}", args.User.Mention)
                .Replace("{OwnerMention}", (await args.Guild.GetOwnerAsync()).Mention)
                .Replace("{UserTag}", args.User.Discriminator)
                .Replace("{MemberCount}", (await args.Guild.GetUsersAsync()).Count.ToString())
                .Replace("{UserString}", args.User.ToString());
            var c = await args.Guild.GetTextChannelAsync(args.Config.WelcomeOptions.WelcomeChannel);

            if (!(c is null))
            {
                var embed = new EmbedBuilder()
                    .WithColor(new Color(args.Config.WelcomeOptions.WelcomeColorR,
                        args.Config.WelcomeOptions.WelcomeColorG,
                        args.Config.WelcomeOptions.WelcomeColorB))
                    .WithDescription(welcomeMessage)
                    .WithThumbnailUrl(args.User.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await embed.SendToAsync(c);
            }
        }

        internal async Task LeaveAsync(UserLeftEventArgs args)
        {
            if (args.Config.WelcomeOptions.LeavingMessage.IsNullOrEmpty()) return;
            var leavingMessage = args.Config.WelcomeOptions.LeavingMessage
                .Replace("{ServerName}", args.Guild.Name)
                .Replace("{UserName}", args.User.Username)
                .Replace("{UserMention}", args.User.Mention)
                .Replace("{OwnerMention}", (await args.Guild.GetOwnerAsync()).Mention)
                .Replace("{UserTag}", args.User.Discriminator)
                .Replace("{MemberCount}", (await args.Guild.GetUsersAsync()).Count.ToString())
                .Replace("{UserString}", args.User.ToString());
            var c = await args.Guild.GetTextChannelAsync(args.Config.WelcomeOptions.WelcomeChannel);
            if (!(c is null))
            {
                var embed = new EmbedBuilder()
                    .WithColor(new Color(
                        args.Config.WelcomeOptions.WelcomeColorR,
                        args.Config.WelcomeOptions.WelcomeColorG,
                        args.Config.WelcomeOptions.WelcomeColorB)
                    )
                    .WithDescription(leavingMessage)
                    .WithThumbnailUrl(args.User.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await embed.SendToAsync(c);
            }
        }
    }
}