using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Gommon;

namespace Volte.Services
{
    public sealed class GuildService : VolteEventService
    {
        private readonly LoggingService _logger;
        private readonly DiscordShardedClient _client;
        private readonly DatabaseService _db;

        public GuildService(LoggingService loggingService,
            DiscordShardedClient discordShardedClient,
            DatabaseService databaseService)
        {
            _logger = loggingService;
            _client = discordShardedClient;
            _db = databaseService;
        }

        public override Task DoAsync(EventArgs args)
        {
            return args switch
            {
                JoinedGuildEventArgs joinedArgs => OnJoinAsync(joinedArgs),
                LeftGuildEventArgs leftArgs => OnLeaveAsync(leftArgs),
                _ => Task.CompletedTask
            };
        }
        

        private async Task OnJoinAsync(JoinedGuildEventArgs args)
        {
            _logger.Debug(LogSource.Volte, "Joined a guild.");
            if (Config.BlacklistedOwners.Contains(args.Guild.Owner.Id))
            {
                _logger.Warn(LogSource.Volte,
                    $"Left guild \"{args.Guild.Name}\" owned by blacklisted owner {args.Guild.Owner}.");
                await args.Guild.LeaveAsync();
                return;
            }

            _ = _db.GetData(args.Guild.Id); //create this guild's configuration if it doesn't already exist (i.e. kicking Volte and reinviting it)
            

            var embed = new EmbedBuilder()
                .WithTitle("Hey there!")
                .WithAuthor(args.Guild.Owner)
                .WithSuccessColor()
                .WithDescription("Thanks for inviting me! Here's some basic instructions on how to set me up.")
                .AddField("Set your admin role", "$adminrole {roleName}", true)
                .AddField("Set your moderator role", "$modrole {roleName}", true)
                .AddField("Permissions", new StringBuilder()
                    .AppendLine("It is recommended to give me admin permission, to avoid any permission errors that may happen.")
                    .AppendLine("You *can* get away with just send messages, ban members, kick members, and the like if you don't want to give me admin.")
                    .ToString())
                .AddField("Support Discord", "[Join my support Discord here](https://discord.gg/H8bcFr2)");

            _logger.Debug(LogSource.Volte,
                "Attempting to send the guild owner the introduction message.");
            try
            {
                await embed.SendToAsync(args.Guild.Owner);
                _logger.Error(LogSource.Volte,
                    "Sent the guild owner the introduction message.");
            }
            catch (HttpException ex) when (ex.HttpCode is HttpStatusCode.Forbidden)
            {
                var c = args.Guild.TextChannels.OrderByDescending(x => x.Position).FirstOrDefault();
                _logger.Error(LogSource.Volte,
                    "Could not DM the guild owner; sending to the upper-most channel instead.");
                if (c != null) await embed.SendToAsync(c);
            }

            if (!Config.GuildLogging.EnsureValidConfiguration(_client, out var channel))
            {
                _logger.Error(LogSource.Volte, "Invalid guild_logging.guild_id/guild_logging.channel_id configuration. Check your IDs and try again.");
                return;
            }

            var all = args.Guild.Users;
            var users = all.Where(u => !u.IsBot).ToList();
            var bots = all.Where(u => u.IsBot).ToList();

            var e = new EmbedBuilder()
                .WithAuthor(args.Guild.Owner)
                .WithTitle("Joined Guild")
                .AddField("Name", args.Guild.Name, true)
                .AddField("ID", args.Guild.Id, true)
                .WithThumbnailUrl(args.Guild.IconUrl)
                .WithCurrentTimestamp()
                .AddField("Users", users.Count(), true)
                .AddField("Bots", bots.Count(), true);

            if (bots.Count() > users.Count())
                await channel.SendMessageAsync(
                    $"{_client.GetOwner().Mention}: Joined a guild with more bots than users.", false,
                    e.WithSuccessColor().Build());
            else
                await e.WithSuccessColor().SendToAsync(channel);
        }

        private async Task OnLeaveAsync(LeftGuildEventArgs args)
        {
            _logger.Debug(LogSource.Volte, "Left a guild.");
            if (!Config.GuildLogging.EnsureValidConfiguration(_client, out var channel))
            {
                _logger.Error(LogSource.Volte, "Invalid guild_logging.guild_id/guild_logging.channel_id configuration. Check your IDs and try again.");
                return;
            }

            await new EmbedBuilder()
                .WithAuthor(args.Guild.Owner)
                .WithTitle("Left Guild")
                .AddField("Name", args.Guild.Name, true)
                .AddField("ID", args.Guild.Id, true)
                .WithThumbnailUrl(args.Guild.IconUrl)
                .WithErrorColor()
                .SendToAsync(channel);
        }
    }
}