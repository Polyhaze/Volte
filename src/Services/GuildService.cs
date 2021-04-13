using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Services
{
    public sealed class GuildService : VolteService
    {
        private readonly DiscordShardedClient _client;

        public GuildService(DiscordShardedClient discordShardedClient) 
            => _client = discordShardedClient;

        public async Task OnJoinAsync(JoinedGuildEventArgs args)
        {
            Logger.Debug(LogSource.Volte, "Joined a guild.");
            if (Config.BlacklistedOwners.Contains(args.Guild.Owner.Id))
            {
                Logger.Warn(LogSource.Volte,
                    $"Left guild \"{args.Guild.Name}\" owned by blacklisted owner {args.Guild.Owner}.");
                await args.Guild.LeaveAsync();
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle("Hey there!")
                .WithAuthor(await _client.Rest.GetUserAsync(Config.Owner))
                .WithColor(Config.SuccessColor)
                .WithDescription("Thanks for inviting me! Here's some basic instructions on how to set me up.")
                .AddField("Set your staff roles", "$setup", true)
                .AddField("Permissions", new StringBuilder()
                    .AppendLine("It is recommended to give me the Administrator permission to avoid any permission errors that may happen.")
                    .AppendLine("You *can* get away with just send messages, ban members, kick members, and the like if you don't want to give me admin; however")
                    .AppendLine("if you're wondering why you're getting permission errors, that's *probably* why.")
                    .ToString())
                .AddField("Support Server", "[Join my support Discord here](https://discord.gg/H8bcFr2)");

            Logger.Debug(LogSource.Volte,
                "Attempting to send the guild owner the introduction message.");
            try
            {
                await embed.SendToAsync(args.Guild.Owner);
                Logger.Error(LogSource.Volte,
                    "Sent the guild owner the introduction message.");
            }
            catch (Exception)
            {
                var c = args.Guild.TextChannels.OrderByDescending(x => x.Position).FirstOrDefault();
                Logger.Error(LogSource.Volte,
                    "Could not DM the guild owner; sending to the upper-most channel instead.");
                if (c != null) await embed.SendToAsync(c);
            }

            if (!Config.GuildLogging.EnsureValidConfiguration(_client, out var channel))
            {
                if (Config.GuildLogging.Enabled)
                    Logger.Error(LogSource.Volte, "Invalid guild_logging.guild_id/guild_logging.channel_id configuration. Check your IDs and try again.");
                return;
            }

            var all = args.Guild.Users;
            var users = all.Where(u => !u.IsBot).ToArray();
            var bots = all.Where(u => u.IsBot).ToArray();

            var e = new EmbedBuilder()
                .WithAuthor(await _client.Rest.GetUserAsync(args.Guild.OwnerId))
                .WithTitle("Joined Guild")
                .AddField("Name", args.Guild.Name, true)
                .AddField("ID", args.Guild.Id, true)
                .WithThumbnailUrl(args.Guild.IconUrl)
                .WithCurrentTimestamp()
                .AddField("Users", users.Length, true)
                .AddField("Bots", bots.Length, true);

            if (bots.Length > users.Length)
                await channel.SendMessageAsync(
                    $"{_client.GetOwner().Mention}: Joined a guild with more bots than users.", false,
                    e.WithSuccessColor().Build());
            else
                await e.WithSuccessColor().SendToAsync(channel);
        }

        public async Task OnLeaveAsync(LeftGuildEventArgs args)
        {
            Logger.Debug(LogSource.Volte, "Left a guild.");
            if (!Config.GuildLogging.EnsureValidConfiguration(_client, out var channel))
            {
                if (Config.GuildLogging.Enabled)
                    Logger.Warn(LogSource.Volte, "Invalid guild_logging.guild_id/guild_logging.channel_id configuration. Check your IDs and try again.");
                return;
            }

            await new EmbedBuilder()
                .WithAuthor(await _client.Rest.GetUserAsync(args.Guild.OwnerId))
                .WithTitle("Left Guild")
                .AddField("Name", args.Guild.Name, true)
                .AddField("ID", args.Guild.Id, true)
                .WithThumbnailUrl(args.Guild.IconUrl)
                .WithErrorColor()
                .SendToAsync(channel);
        }
    }
}