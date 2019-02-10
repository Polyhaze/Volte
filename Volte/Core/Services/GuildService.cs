using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Data;
using Volte.Core.Data.Objects;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Services {
    public class GuildService {
        public async Task OnJoin(SocketGuild guild) {
            /*if (Config.GetBlacklistedOwners().Contains(guild.OwnerId)) {
                await guild.LeaveAsync();
                return;
            }*/

            var embed = new EmbedBuilder()
                .WithTitle("Hey there!")
                .WithAuthor(guild.Owner)
                .WithColor(Config.GetSuccessColor())
                .WithDescription("Thanks for inviting me! Here's some basic instructions on how to set me up.")
                .AddField("Set your admin role", "$adminrole {roleName}", true)
                .AddField("Set your moderator role", "$modrole {roleName}", true)
                .AddField("Permissions",
                    "It is recommended to give me admin permission, to avoid any permission errors that may happen." +
                    "\nYou *can* get away with just send messages, ban members, kick members, and the like if you don't want to give me admin.")
                .AddField("Support Server", "[Join my support Discord here](https://discord.gg/H8bcFr2)");

            try {
                await embed.SendTo(guild.Owner);
            }
            catch (HttpException e) when (e.DiscordCode.Equals(50007)) {
                var c = guild.TextChannels?.First();
                if (c != null) {
                    await embed.SendTo(c);
                }
            }

            if (Config.GetJoinLeaveLog().Enabled) {
                var joinLeave = Config.GetJoinLeaveLog();
                var logger = VolteBot.ServiceProvider.GetRequiredService<LoggingService>();
                if (joinLeave.GuildId.Equals(0) || joinLeave.ChannelId.Equals(0)) {
                    logger.Log(LogSeverity.Error, LogSource.Service,
                        "Invalid value set for the GuildId or ChannelId in the JoinLeaveLog config option. " +
                        "To fix, set Enabled to false, or correctly fill in your options.");
                    return;
                }

                var channel = VolteBot.Client.GetGuild(joinLeave.GuildId).GetTextChannel(joinLeave.ChannelId);
                var users = guild.Users.Where(u => !u.IsBot).ToList();
                var bots = guild.Users.Where(u => u.IsBot).ToList();

                var e = new EmbedBuilder()
                    .WithAuthor(guild.Owner)
                    .WithTitle("Joined Guild")
                    .AddField("Name", guild.Name, true)
                    .AddField("ID", guild.Id, true)
                    .WithThumbnailUrl(guild.IconUrl)
                    .WithCurrentTimestamp()
                    .AddField("Users", users.Count, true)
                    .AddField("Bots", bots.Count, true);
                try {
                    if (bots.Count > users.Count) {
                        await channel.SendMessageAsync(
                            $"<@{Config.GetOwner()}>: Joined a guild with more bots than users.", false,
                            e.WithColor(0x00FF00).Build());
                    }
                    else {
                        await channel.SendMessageAsync("", false, e.WithColor(0x00FF00).Build());
                    }
                }
                catch (NullReferenceException ex) {
                    logger.Log(LogSeverity.Error, LogSource.Service,
                        "Invalid JoinLeaveLog.GuildId/JoinLeaveLog.ChannelId configuration.", ex);
                }
            }
        }

        public async Task OnLeave(SocketGuild guild) {
            if (Config.GetJoinLeaveLog().Enabled) {
                var logger = VolteBot.ServiceProvider.GetRequiredService<LoggingService>();
                var joinLeave = Config.GetJoinLeaveLog();
                if (joinLeave.GuildId.Equals(0) || joinLeave.ChannelId.Equals(0)) {
                    logger.Log(LogSeverity.Error, LogSource.Service,
                        "Invalid value set for the GuildId or ChannelId in the JoinLeaveLog config option. " +
                        "To fix, set Enabled to false, or correctly fill in your options.");
                    return;
                }
                
                var channel = VolteBot.Client.GetGuild(joinLeave.GuildId).GetTextChannel(joinLeave.ChannelId);
                try {
                    var e = new EmbedBuilder()
                        .WithAuthor(guild.Owner)
                        .WithTitle("Left Guild")
                        .AddField("Name", guild.Name, true)
                        .AddField("ID", guild.Id, true)
                        .WithThumbnailUrl(guild.IconUrl)
                        .WithColor(0xFF0000)
                        .SendTo(channel);
                }
                catch (NullReferenceException e) {
                    logger.Log(LogSeverity.Error, LogSource.Service,
                        "Invalid JoinLeaveLog.GuildId/JoinLeaveLog.ChannelId configuration.", e);
                }
            }
            
            
        }
    }
}