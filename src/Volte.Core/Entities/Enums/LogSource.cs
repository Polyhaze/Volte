using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Volte.Core.Entities
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LogSourceAttribute : Attribute
    {
        public static IReadOnlyDictionary<LogSource, LogSourceAttribute> LogSources { get; }
        public static IReadOnlyDictionary<int, LogSource> EventIdMappings { get; }
        
        static LogSourceAttribute()
        {
            var logSources = new Dictionary<LogSource, LogSourceAttribute>();
            foreach (LogSource logSource in Enum.GetValues(typeof(LogSource)))
            {
                var name = logSource.ToString();
                var attr = (LogSourceAttribute) typeof(LogSource)
                    .GetField(name)!
                    .GetCustomAttribute(typeof(LogSourceAttribute));
                attr.Name ??= name;
                logSources[logSource] = attr;
            }
            LogSources = logSources;
            
            var eventIdMappings = new Dictionary<int, LogSource>();
            foreach (var (logSource, attr) in LogSources)
            {
                if (attr.MapsToEventId != 0)
                {
                    eventIdMappings[attr.MapsToEventId] = logSource;
                }
            }
            EventIdMappings = eventIdMappings;
        }
        
        public Color Color { get; }

        [CanBeNull] private string _name;
        [CanBeNull]
        public string Name
        {
            get => _name;
            set => _name = value.ToUpperInvariant();
        }

        public int MapsToEventId { get; set; }

        public LogSourceAttribute(KnownColor color)
        {
            Color = Color.FromKnownColor(color);
        }
    }
    
    public enum LogSource : uint
    {
        /// <summary>
        ///     Indicates that this log message is from a command or module.
        /// </summary>
        [LogSource(KnownColor.LimeGreen)]
        Module = 1,
        /// <summary>
        ///     Indicates that this log message is from a Service.
        /// </summary>
        [LogSource(KnownColor.Gold)]
        Service = 2,
        /// <summary>
        ///     Indicates that this log message is from Discord.Net.
        /// </summary>
        [LogSource(KnownColor.RoyalBlue)]
        Discord = 3,
        /// <summary>
        ///     Indicates that this log message is from the Discord HTTP REST API.
        /// </summary>
        [LogSource(KnownColor.Red)]
        Rest = 4,
        /// <summary>
        ///     Indicates that this log message is from the Discord WebSocket Gateway connection.
        /// </summary>
        [LogSource(KnownColor.RoyalBlue, Name = nameof(Discord))]
        Gateway = 5,
        /// <summary>
        ///     Indicates that this log message is from the Volte itself.
        /// </summary>
        [LogSource(KnownColor.LawnGreen)]
        Volte = 6,
        
        [LogSource(KnownColor.Green, Name = "Interactive")]
        Interactivity = 7,
        
        [LogSource(KnownColor.Aqua)]
        AutoShard = 8,

        [LogSource(KnownColor.Indigo)]
        WebSocket = 9,
        
        [LogSource(KnownColor.Indigo, Name = nameof(WebSocket))]
        WebSocketDispatch = 10,
        
        [LogSource(KnownColor.Aquamarine)]
        DSharpPlus = 11,
        
        MaxValue = DSharpPlus,
        
        /// <summary>
        /// Miscellaneous events, that do not fit in any other category.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 100, Name = "DSharpPlus")] Misc = MaxValue + 100,

        /// <summary>
        /// Events pertaining to startup tasks.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 101)] Startup = MaxValue + 101,

        /// <summary>
        /// Events typically emitted whenever WebSocket connections fail or are terminated.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 102)] ConnectionFailure = MaxValue + 102,

        /// <summary>
        /// Events pertaining to Discord-issued session state updates.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 103)] SessionUpdate = MaxValue + 103,

        /// <summary>
        /// Events emitted when exceptions are thrown in handlers attached to async events.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 104)] EventHandlerException = MaxValue + 104,

        /// <summary>
        /// Events emitted for various high-level WebSocket receive events.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 105)] WebSocketReceive = MaxValue + 105,

        /// <summary>
        /// Events emitted for various low-level WebSocket receive events.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 106)] WebSocketReceiveRaw = MaxValue + 106,

        /// <summary>
        /// Events emitted for various low-level WebSocket send events.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 107)] WebSocketSendRaw = MaxValue + 107,

        /// <summary>
        /// Events emitted for various WebSocket payload processing failures, typically when deserialization or decoding fails.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 108)] WebSocketReceiveFailure = MaxValue + 108,

        /// <summary>
        /// Events pertaining to connection lifecycle, specifically, heartbeats.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 109)] Heartbeat = MaxValue + 109,

        /// <summary>
        /// Events pertaining to various heartbeat failures, typically fatal.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 110)] HeartbeatFailure = MaxValue + 110,

        /// <summary>
        /// Events pertaining to clean connection closes.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 111)] ConnectionClose = MaxValue + 111,

        /// <summary>
        /// Events emitted when REST processing fails for any reason.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 112)] RestError = MaxValue + 112,

        /// <summary>
        /// Events pertaining to autosharded client shard startup.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 113)] ShardStartup = MaxValue + 113,

        /// <summary>
        /// Events pertaining to ratelimit exhaustion.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 114)] RatelimitHit = MaxValue + 114,

        /// <summary>
        /// Events pertaining to ratelimit diagnostics. Typically contain raw bucket info.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 115)] RatelimitDiag = MaxValue + 115,

        /// <summary>
        /// Events emitted when a ratelimit is exhausted and a request is preemtively blocked.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 116)] RatelimitPreemptive = MaxValue + 116,

        /// <summary>
        /// Events pertaining to audit log processing.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 117)] AuditLog = MaxValue + 117,

        /// <summary>
        /// Events containing raw (but decompressed) payloads, received from Discord Gateway.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 118)] GatewayWsRx = MaxValue + 118,

        /// <summary>
        /// Events containing raw payloads, as they're being sent to Discord Gateway.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 119)] GatewayWsTx = MaxValue + 119,

        /// <summary>
        /// Events pertaining to Gateway Intents. Typically diagnostic information.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 120)] Intents = MaxValue + 120,

        /// <summary>
        /// Events pertaining to autosharded client shard shutdown, clean or otherwise.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 121)] ShardShutdown = MaxValue + 121,

        /// <summary>
        /// Events containing raw payloads, as they're received from Discord's REST API.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 122)] RestRx = MaxValue + 122,

        /// <summary>
        /// Events containing raw payloads, as they're sent to Discord's REST API.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 123)] RestTx = MaxValue + 123,
        
        /// <summary>
        ///     Indicates that this log message came from an unknown source.
        /// </summary>
        [LogSource(KnownColor.Red)]
        Unknown = uint.MaxValue
    }
}