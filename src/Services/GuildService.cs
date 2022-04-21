using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte;
using Volte.Entities;
using Volte.Helpers;

namespace Volte.Services
{
    public sealed class GuildService : IVolteService
    {
        private readonly DiscordShardedClient _client;
        private readonly InteractionService _interactions;

        public GuildService(DiscordShardedClient discordShardedClient, InteractionService interactionService)
        {
            _client = discordShardedClient;
            _interactions = interactionService;
            _client.JoinedGuild += async g => await OnJoinAsync(new JoinedGuildEventArgs(g));
            _client.LeftGuild += async g => await OnLeaveAsync(new LeftGuildEventArgs(g));
        }

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
                .AddField("Set your staff roles", "Run `/settings admin-role` or `/settings mod-role` in your server!", true)
                .AddField("Permissions", new StringBuilder()
                    .AppendLine("It is recommended to give me the Administrator permission to avoid any permission errors that may happen.")
                    .AppendLine("You *can* get away with just send messages, ban members, kick members, and the like if you don't want to give me admin; however")
                    .AppendLine("if you're wondering why you're getting permission errors, that's *probably* why.")
                    .ToString())
                .AddField("Support Server", "https://discord.gg/H8bcFr2");

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
                if (c != null)
                    await c.TrySendMessageAsync(embed: embed.Build());
            }

            if (!Config.GuildLogging.TryValidate(_client, out var channel))
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

            if (bots.Length > users.Length && !(bots.Length > 5))
                await channel.SendMessageAsync(
                    $"{_client.GetOwner().Mention}: Joined a guild with more bots than users.", embed: e.WithSuccessColor().Build());
            else
                await e.WithSuccessColor().SendToAsync(channel);

            await _interactions.CommandUpdater.UpsertMissingCommandsAsync(args.Guild.Id);

        }

        public async Task OnLeaveAsync(LeftGuildEventArgs args)
        {
            Logger.Debug(LogSource.Volte, "Left a guild.");
            if (!Config.GuildLogging.TryValidate(_client, out var channel))
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