using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Volte.Core.Data;
using Volte.Core.Data.Models;
using Volte.Core.Data.Models.EventArgs;
using Gommon;

namespace Volte.Services
{
    [Service("Guild", "The main Service that handles guild-related Discord gateway events.")]
    public sealed class GuildService
    {
        private readonly LoggingService _logger;
        private readonly DiscordShardedClient _client;

        public GuildService(LoggingService loggingService,
            DiscordShardedClient discordShardedClient)
        {
            _logger = loggingService;
            _client = discordShardedClient;
        }

        public async Task OnJoinAsync(JoinedGuildEventArgs args)
        {
            await _logger.LogAsync(LogSeverity.Debug, LogSource.Volte, "Joined a guild.");
            if (Config.BlacklistedOwners.Contains(args.Guild.Owner.Id))
            {
                await _logger.LogAsync(LogSeverity.Warning, LogSource.Volte,
                    $"Left guild \"{args.Guild.Name}\" owned by blacklisted owner {args.Guild.Owner}.");
                await args.Guild.LeaveAsync();
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle("Hey there!")
                .WithAuthor(args.Guild.Owner)
                .WithColor(Config.SuccessColor)
                .WithDescription("Thanks for inviting me! Here's some basic instructions on how to set me up.")
                .AddField("Set your admin role", "$adminrole {roleName}", true)
                .AddField("Set your moderator role", "$modrole {roleName}", true)
                .AddField("Permissions",
                    "It is recommended to give me admin permission, to avoid any permission errors that may happen." +
                    "\nYou *can* get away with just send messages, ban members, kick members, and the like if you don't want to give me admin.")
                .AddField("Support Server", "[Join my support Discord here](https://discord.gg/H8bcFr2)");

            await _logger.LogAsync(LogSeverity.Debug, LogSource.Volte,
                "Attempting to send the guild owner the introduction message.");
            try
            {
                await embed.SendToAsync(args.Guild.Owner);
                await _logger.LogAsync(LogSeverity.Debug, LogSource.Volte,
                    "Sent the guild owner the introduction message.");
            }
            catch (HttpException ex) when (ex.DiscordCode is 50007)
            {
                var c = args.Guild.TextChannels.OrderByDescending(x => x.Position).FirstOrDefault();
                await _logger.LogAsync(LogSeverity.Debug, LogSource.Volte,
                    "Could not DM the guild owner; sending to the upper-most channel instead.");
                if (c != null) await embed.SendToAsync(c);
            }

            if (!Config.JoinLeaveLog.Enabled) return;
            var joinLeave = Config.JoinLeaveLog;
            if (joinLeave.GuildId is 0 || joinLeave.ChannelId is 0)
            {
                await _logger.LogAsync(LogSeverity.Error, LogSource.Volte,
                    "Invalid value set for the GuildId or ChannelId in the JoinLeaveLog config option. " +
                    "To fix, set Enabled to false, or correctly fill in your options.");
                return;
            }

            var channel = _client.GetGuild(joinLeave.GuildId).GetTextChannel(joinLeave.ChannelId);
            if (channel is null)
            {
                await _logger.LogAsync(LogSeverity.Error, LogSource.Volte,
                    "Invalid JoinLeaveLog.GuildId/JoinLeaveLog.ChannelId configuration.");
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
                await channel.SendMessageAsync("", false, e.WithSuccessColor().Build());
        }

        public async Task OnLeaveAsync(LeftGuildEventArgs args)
        {
            await _logger.LogAsync(LogSeverity.Debug, LogSource.Volte, "Left a guild.");
            if (!Config.JoinLeaveLog.Enabled) return;
            var joinLeave = Config.JoinLeaveLog;
            if (joinLeave.GuildId is 0 || joinLeave.ChannelId is 0)
            {
                await _logger.LogAsync(LogSeverity.Error, LogSource.Volte,
                    "Invalid value set for the GuildId or ChannelId in the JoinLeaveLog config option. " +
                    "To fix, set Enabled to false, or correctly fill in your options.");
                return;
            }

            var channel = _client.GetGuild(joinLeave.GuildId).GetTextChannel(joinLeave.ChannelId);
            if (channel is null)
            {
                await _logger.LogAsync(LogSeverity.Error, LogSource.Volte,
                    "Invalid JoinLeaveLog.GuildId/JoinLeaveLog.ChannelId configuration.");
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