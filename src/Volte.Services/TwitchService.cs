using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using Qommon.Collections;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using Volte.Core.Entities;

namespace Volte.Services
{
    public class TwitchService : VolteService
    {
        private readonly DiscordShardedClient _client;
        private readonly DatabaseService _db;
        private Dictionary<string, ulong> _toMonitor;
        private readonly LiveStreamMonitorService _monitor;

        public TwitchService(DiscordShardedClient discordShardedClient,
            DatabaseService databaseService, LiveStreamMonitorService liveStreamMonitorService)
        {
            _client = discordShardedClient;
            _db = databaseService;
            _toMonitor = new Dictionary<string, ulong>();
            _monitor = liveStreamMonitorService;
        }

        public async Task InitializeAsync()
        {
            var result = new Dictionary<string, ulong>();
            foreach (var guild in _client.ShardClients.Select(x => x.Value).SelectMany(x => x.Guilds))
            {
                var data = _db.GetData(guild.Key);
                foreach (var streamer in data.Configuration.Twitch.Streamers.Where(streamer => !result.Keys.ContainsIgnoreCase(streamer)))
                {
                    result.Add(streamer, data.Id);
                }
            }

            _toMonitor = result;
            _monitor.SetChannelsByName(result.Keys.ToList());
        }
        // ReSharper disable twice RedundantAssignment
        public void HandleStreamOnline(object _, OnStreamOnlineArgs args)
        {
            _ = Task.Run(async () => await PostToChannelsAsync(args.Channel, args.Stream, true));
        }
        
        public void HandleStreamOffline(object _, OnStreamOfflineArgs args)
        {
            _ = Task.Run(async () => await PostToChannelsAsync(args.Channel, args.Stream, false));
        }

        public async Task PostToChannelsAsync(string channel, Stream stream, bool online)
        {
            var guildsToPostTo = _toMonitor.Where(x => x.Key.EqualsIgnoreCase(channel))
                .Select(x => x.Value)
                .Select(x => _client.GetGuild(x));

            foreach (var guild in guildsToPostTo)
            {
                var sb = new StringBuilder();
                var data = _db.GetData(guild.Id);
                switch (data.Configuration.Twitch.NotificationType)
                {
                    case NotificationType.MentionEveryone:
                        sb.Append("@everyone");
                        break;
                    case NotificationType.Nothing:
                        break;
                    case NotificationType.MentionHere:
                        sb.Append("@here");
                        break;
                }
                
                var embed = new DiscordEmbedBuilder()
                    .WithSuccessColor()
                    .WithTimestamp(stream.StartedAt)
                    .WithImageUrl(stream.ThumbnailUrl)
                    .WithAuthor(stream.ViewerCount.ToString(), iconUrl: guild.IconUrl)
                    .WithTitle(stream.Title)
                    .AddField("Language", stream.Language);

                sb.Append(online ? $"{channel} is now live!" : $"{channel} is no longer live!");

                await guild.GetChannel(data.Configuration.Twitch.ChannelId).SendMessageAsync(sb.ToString(), embed: embed.Build());
            }
        }
    }
}