using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Volte.Core;
using Volte.Data;
using Volte.Data.Models;
using Volte.Data.Models.EventArgs;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("Guild", "The main Service that handles guild-related Discord gateway events.")]
    public sealed class GuildService
    {
        private readonly LoggingService _logger;
        private readonly DiscordSocketClient _client;

        public GuildService(LoggingService loggingService,
            DiscordSocketClient discordSocketClient)
        {
            _logger = loggingService;
            _client = discordSocketClient;
        }

        public async Task OnJoinAsync(JoinedGuildEventArgs args)
        {
            var owner = await args.Guild.GetOwnerAsync();
            if (Config.BlacklistedOwners.Contains(owner.Id))
            {
                await _logger.LogAsync(LogSeverity.Warning, LogSource.Volte,
                    $"Left guild \"{args.Guild.Name}\" owned by blacklisted owner {await args.Guild.GetOwnerAsync()}.");
                await args.Guild.LeaveAsync();
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle("Hey there!")
                .WithAuthor(owner)
                .WithColor(Config.SuccessColor)
                .WithDescription("Thanks for inviting me! Here's some basic instructions on how to set me up.")
                .AddField("Set your admin role", "$adminrole {roleName}", true)
                .AddField("Set your moderator role", "$modrole {roleName}", true)
                .AddField("Permissions",
                    "It is recommended to give me admin permission, to avoid any permission errors that may happen." +
                    "\nYou *can* get away with just send messages, ban members, kick members, and the like if you don't want to give me admin.")
                .AddField("Support Server", "[Join my support Discord here](https://discord.gg/H8bcFr2)");

            try
            {
                await embed.SendToAsync(owner);
            }
            catch (HttpException ex) when (ex.DiscordCode is 50007)
            {
                var c = (await args.Guild.GetTextChannelsAsync()).OrderByDescending(x => x.Position).FirstOrDefault();
                if (c != null) await embed.SendToAsync(c);
            }

            if (!Config.JoinLeaveLog.Enabled) return;
            var joinLeave = Config.JoinLeaveLog;
            if (joinLeave.GuildId is 0 || joinLeave.ChannelId is 0)
            {
                await _logger.LogAsync(LogSeverity.Error, LogSource.Service,
                    "Invalid value set for the GuildId or ChannelId in the JoinLeaveLog config option. " +
                    "To fix, set Enabled to false, or correctly fill in your options.");
                return;
            }

            var channel = _client.GetGuild(joinLeave.GuildId).GetTextChannel(joinLeave.ChannelId);
            if (channel is null)
            {
                await _logger.LogAsync(LogSeverity.Error, LogSource.Service,
                    "Invalid JoinLeaveLog.GuildId/JoinLeaveLog.ChannelId configuration.");
                return;
            }

            var all = await args.Guild.GetUsersAsync();
            var users = all.Where(u => !u.IsBot);
            var bots = all.Where(u => u.IsBot);

            var e = new EmbedBuilder()
                .WithAuthor(owner)
                .WithTitle("Joined Guild")
                .AddField("Name", args.Guild.Name, true)
                .AddField("ID", args.Guild.Id, true)
                .WithThumbnailUrl(args.Guild.IconUrl)
                .WithCurrentTimestamp()
                .AddField("Users", users.Count(), true)
                .AddField("Bots", bots.Count(), true);

            if (bots.Count() > users.Count())
                await channel.SendMessageAsync(
                    $"<@{Config.Owner}>: Joined a guild with more bots than users.", false,
                    e.WithSuccessColor().Build());
            else
                await channel.SendMessageAsync("", false, e.WithSuccessColor().Build());
        }

        public async Task OnLeaveAsync(LeftGuildEventArgs args)
        {
            if (!Config.JoinLeaveLog.Enabled) return;
            var joinLeave = Config.JoinLeaveLog;
            if (joinLeave.GuildId is 0 || joinLeave.ChannelId is 0)
            {
                await _logger.LogAsync(LogSeverity.Error, LogSource.Service,
                    "Invalid value set for the GuildId or ChannelId in the JoinLeaveLog config option. " +
                    "To fix, set Enabled to false, or correctly fill in your options.");
                return;
            }

            var channel = _client.GetGuild(joinLeave.GuildId).GetTextChannel(joinLeave.ChannelId);
            if (channel is null)
            {
                await _logger.LogAsync(LogSeverity.Error, LogSource.Service,
                    "Invalid JoinLeaveLog.GuildId/JoinLeaveLog.ChannelId configuration.");
                return;
            }

            await new EmbedBuilder()
                .WithAuthor(await args.Guild.GetOwnerAsync())
                .WithTitle("Left Guild")
                .AddField("Name", args.Guild.Name, true)
                .AddField("ID", args.Guild.Id, true)
                .WithThumbnailUrl(args.Guild.IconUrl)
                .WithErrorColor()
                .SendToAsync(channel);

        }
    }
}
