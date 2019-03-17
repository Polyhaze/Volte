using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using Volte.Data;
using Volte.Data.Objects;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("Guild", "The main Service that handles guild-related Discord gateway events.")]
    public sealed class GuildService
    {
        private readonly LoggingService _logger;

        public GuildService(LoggingService loggingService)
        {
            _logger = loggingService;
        }

        public async Task OnJoinAsync(GuildCreateEventArgs args)
        {
            if (Config.BlacklistedOwners.Contains(args.Guild.Owner.Id))
            {
                await _logger.Log(LogLevel.Warning, LogSource.Volte,
                    $"Left guild \"{args.Guild.Name}\" owned by blacklisted owner {args.Guild.Owner.ToHumanReadable()}.");
                await args.Guild.LeaveAsync();
                return;
            }

            var owner = args.Guild.Owner;

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Hey there!")
                .WithAuthor(owner)
                .WithSuccessColor()
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
            catch (UnauthorizedException)
            {
                var c = args.Guild.Channels.FirstOrDefault();
                if (c != null) await embed.SendToAsync(c);
            }

            if (Config.JoinLeaveLog.Enabled)
            {
                var joinLeave = Config.JoinLeaveLog;
                var logger = VolteBot.GetRequiredService<LoggingService>();
                if (joinLeave.GuildId.Equals(0) || joinLeave.ChannelId.Equals(0))
                {
                    await logger.Log(LogLevel.Error, LogSource.Service,
                        "Invalid value set for the GuildId or ChannelId in the JoinLeaveLog config option. " +
                        "To fix, set Enabled to false, or correctly fill in your options.");
                    return;
                }

                var channel = VolteBot.Client.Guilds.First(x => x.Value.Id == joinLeave.GuildId).Value.Channels
                    .First(x => x.Id == joinLeave.ChannelId);
                var users = args.Guild.Members.Where(u => !u.IsBot).ToList();
                var bots = args.Guild.Members.Where(u => u.IsBot).ToList();

                var e = new DiscordEmbedBuilder()
                    .WithAuthor(owner)
                    .WithTitle("Joined Guild")
                    .AddField("Name", args.Guild.Name, true)
                    .AddField("ID", args.Guild.Id.ToString(), true)
                    .WithThumbnailUrl(args.Guild.IconUrl)
                    .WithTimestamp(DateTime.UtcNow)
                    .AddField("Users", users.Count.ToString(), true)
                    .AddField("Bots", bots.Count.ToString(), true);
                try
                {
                    if (bots.Count > users.Count)
                        await channel.SendMessageAsync(
                            $"<@{Config.Owner}>: Joined a guild with more bots than users.", false,
                            e.WithErrorColor().Build());
                    else
                        await channel.SendMessageAsync("", false, e.WithSuccessColor().Build());
                }
                catch (NullReferenceException ex)
                {
                    await logger.Log(LogLevel.Error, LogSource.Service,
                        "Invalid JoinLeaveLog.GuildId/JoinLeaveLog.ChannelId configuration.", ex);
                }
            }
        }

        public async Task OnLeaveAsync(GuildDeleteEventArgs args)
        {
            if (Config.JoinLeaveLog.Enabled)
            {
                var logger = VolteBot.GetRequiredService<LoggingService>();
                var joinLeave = Config.JoinLeaveLog;
                if (joinLeave.GuildId.Equals(0) || joinLeave.ChannelId.Equals(0))
                {
                    await logger.Log(LogLevel.Error, LogSource.Service,
                        "Invalid value set for the GuildId or ChannelId in the JoinLeaveLog config option. " +
                        "To fix, set Enabled to false, or correctly fill in your options.");
                    return;
                }

                var channel = VolteBot.Client.Guilds.First(x => x.Value.Id == joinLeave.GuildId).Value.Channels
                    .First(x => x.Id == joinLeave.ChannelId);
                try
                {
                    var e = new DiscordEmbedBuilder()
                        .WithAuthor(args.Guild.Owner)
                        .WithTitle("Left Guild")
                        .AddField("Name", args.Guild.Name, true)
                        .AddField("ID", args.Guild.Id.ToString(), true)
                        .WithThumbnailUrl(args.Guild.IconUrl)
                        .WithErrorColor()
                        .SendToAsync(channel);
                }
                catch (NullReferenceException e)
                {
                    await logger.Log(LogLevel.Error, LogSource.Service,
                        "Invalid JoinLeaveLog.GuildId/JoinLeaveLog.ChannelId configuration.", e);
                }
            }
        }
    }
}