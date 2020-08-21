using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Gommon;
using Volte.Core.Helpers;
using Volte.Interactive;

namespace Volte.Services
{
    public sealed class GuildService : VolteEventService
    {
        private readonly LoggingService _logger;
        private readonly DiscordShardedClient _client;
        private readonly DatabaseService _db;
        private readonly InteractiveService _interactive;

        public GuildService(LoggingService loggingService,
            DiscordShardedClient discordShardedClient,
            DatabaseService databaseService,
            InteractiveService interactiveService)
        {
            _logger = loggingService;
            _client = discordShardedClient;
            _db = databaseService;
            _interactive = interactiveService;
        }

        public override Task DoAsync(EventArgs args)
        {
            return args switch
            {
                GuildCreateEventArgs joinedArgs => OnJoinAsync(joinedArgs),
                GuildDeleteEventArgs leftArgs => OnLeaveAsync(leftArgs),
                _ => Task.CompletedTask
            };
        }
        

        private async Task OnJoinAsync(GuildCreateEventArgs args)
        {
            _logger.Debug(LogSource.Volte, "Joined a guild.");
            if (Config.BlacklistedOwners.Contains(args.Guild.Owner.Id))
            {
                _logger.Warn(LogSource.Volte,
                    $"Left guild \"{args.Guild.Name}\" owned by blacklisted owner {args.Guild.Owner}.");
                await args.Guild.LeaveAsync();
                return;
            }

            var data = _db.GetData(args.Guild.Id); //create this guild's configuration if it doesn't already exist (i.e. kicking Volte and reinviting it)
            
            
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Hey there!")
                .WithSuccessColor()
                .WithDescription("Thanks for inviting me! Here's some basic instructions on how to set me up.")
                .AddField("Set your admin role", $"{data.Configuration.CommandPrefix}adminrole {{roleName}}", true)
                .AddField("Set your moderator role", $"{data.Configuration.CommandPrefix}modrole {{roleName}}", true)
                .AddField("Permissions", new StringBuilder()
                    .AppendLine("It is recommended to give me admin permission, to avoid any permission errors that may happen.")
                    .AppendLine("You *can* get away with just send messages, ban members, kick members, and the like if you don't want to give me admin.")
                    .AppendLine("However, issues can arise from not giving me admin. If I'm giving you permission errors, that's a quick fix.")
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
            catch (UnauthorizedException)
            {
                var c = args.Guild.Channels.OrderByDescending(x => x.Value.Position).FirstOrDefault().Value;
                _logger.Error(LogSource.Volte,
                    "Could not DM the guild owner; sending to the upper-most channel instead.");
                if (c is not null) await embed.SendToAsync(c);
            }

            if (!Config.GuildLogging.EnsureValidConfiguration(_client, out var channel))
            {
                _logger.Error(LogSource.Volte, "Invalid guild_logging.guild_id/guild_logging.channel_id configuration. Check your IDs and try again.");
                return;
            }

            var all = args.Guild.Members.Select(x => x.Value).ToList();
            var users = all.Where(u => !u.IsBot).ToList();
            var bots = all.Where(u => u.IsBot).ToList();

            var e = new DiscordEmbedBuilder()
                .WithAuthor(args.Guild.Owner.GetEffectiveUsername(), iconUrl: args.Guild.Owner.GetAvatarUrl(ImageFormat.Auto))
                .WithTitle("Joined Guild")
                .AddField("Name", args.Guild.Name, true)
                .AddField("ID", args.Guild.Id, true)
                .WithThumbnail(args.Guild.IconUrl)
                .WithTimestamp(DateTimeOffset.Now)
                .AddField("Users", users.Count, true)
                .AddField("Bots", bots.Count, true);

            if (bots.Count > users.Count)
            {
                await SendMessageWithGuildLeaveOptionAsync(channel,
                    $"<@{Config.Owner}>: Joined a guild with more bots than users. Possibly a bot farm?",
                    e.WithSuccessColor().Build(), args.Guild);
            }
            else
                await e.WithSuccessColor().SendToAsync(channel);
        }

        private async Task OnLeaveAsync(GuildDeleteEventArgs args)
        {
            _logger.Debug(LogSource.Volte, "Left a guild.");
            if (!Config.GuildLogging.EnsureValidConfiguration(_client, out var channel))
            {
                _logger.Error(LogSource.Volte, "Invalid guild_logging.guild_id/guild_logging.channel_id configuration. Check your IDs and try again.");
                return;
            }
            

            await new DiscordEmbedBuilder()
                .WithTitle("Left Guild")
                .AddField("Name", args.Guild.Name, true)
                .AddField("ID", args.Guild.Id, true)
                .WithThumbnail(args.Guild.IconUrl)
                .WithErrorColor()
                .SendToAsync(channel);
        }

        private async Task<DiscordMessage> SendMessageWithGuildLeaveOptionAsync(DiscordChannel channel, string content, DiscordEmbed embed, DiscordGuild guild,
            TimeSpan? timeout = null)
        {
            var m = await channel.SendMessageAsync(content, embed: embed);
            await m.CreateReactionAsync(EmojiHelper.X.ToEmoji());
            _interactive.AddReactionCallback(m, new LeaveGuildReactionCallback(guild));
            return m;
        }
    }
}