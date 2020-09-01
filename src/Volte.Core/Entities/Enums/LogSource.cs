using System.Drawing;

namespace Volte.Core.Entities
{

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
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 100, Name = "DSharpPlus")] 
        Misc = MaxValue + 100,

        /// <summary>
        /// Events pertaining to startup tasks.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 101, Name = "Startup")] 
        Startup = MaxValue + 101,

        /// <summary>
        /// Events typically emitted whenever WebSocket connections fail or are terminated.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 102, Name = "Gateway")] 
        ConnectionFailure = MaxValue + 102,

        /// <summary>
        /// Events pertaining to Discord-issued session state updates.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 103, Name = "WebSocket")] 
        SessionUpdate = MaxValue + 103,

        /// <summary>
        /// Events emitted when exceptions are thrown in handlers attached to async events.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 104, Name = "Event")] 
        EventHandlerException = MaxValue + 104,

        /// <summary>
        /// Events emitted for various high-level WebSocket receive events.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 105, Name = "WebSocket")] 
        WebSocketReceive = MaxValue + 105,

        /// <summary>
        /// Events emitted for various low-level WebSocket receive events.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 106, Name = "WebSocket")] 
        WebSocketReceiveRaw = MaxValue + 106,

        /// <summary>
        /// Events emitted for various low-level WebSocket send events.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 107, Name = "WebSocket")] 
        WebSocketSendRaw = MaxValue + 107,

        /// <summary>
        /// Events emitted for various WebSocket payload processing failures, typically when deserialization or decoding fails.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 108, Name = "WebSocket")] 
        WebSocketReceiveFailure = MaxValue + 108,

        /// <summary>
        /// Events pertaining to connection lifecycle, specifically, heartbeats.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 109, Name = "Heartbeat")] 
        Heartbeat = MaxValue + 109,

        /// <summary>
        /// Events pertaining to various heartbeat failures, typically fatal.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 110, Name = "Heartbeat")] 
        HeartbeatFailure = MaxValue + 110,

        /// <summary>
        /// Events pertaining to clean connection closes.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 111, Name = "Gateway")] 
        ConnectionClose = MaxValue + 111,

        /// <summary>
        /// Events emitted when REST processing fails for any reason.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 112, Name = "Rest")] 
        RestError = MaxValue + 112,

        /// <summary>
        /// Events pertaining to autosharded client shard startup.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 113, Name = "Shard")] 
        ShardStartup = MaxValue + 113,

        /// <summary>
        /// Events pertaining to ratelimit exhaustion.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 114, Name = "Ratelimiter")] 
        RatelimitHit = MaxValue + 114,

        /// <summary>
        /// Events pertaining to ratelimit diagnostics. Typically contain raw bucket info.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 115, Name = "Ratelimiter")] 
        RatelimitDiag = MaxValue + 115,

        /// <summary>
        /// Events emitted when a ratelimit is exhausted and a request is preemtively blocked.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 116, Name = "Ratelimiter")] 
        RatelimitPreemptive = MaxValue + 116,

        /// <summary>
        /// Events pertaining to audit log processing.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 117)] 
        AuditLog = MaxValue + 117,

        /// <summary>
        /// Events containing raw (but decompressed) payloads, received from Discord Gateway.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 118, Name = "Gateway")] 
        GatewayWsRx = MaxValue + 118,

        /// <summary>
        /// Events containing raw payloads, as they're being sent to Discord Gateway.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 119, Name = "Gateway")] 
        GatewayWsTx = MaxValue + 119,

        /// <summary>
        /// Events pertaining to Gateway Intents. Typically diagnostic information.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 120, Name = "Intents")] 
        Intents = MaxValue + 120,

        /// <summary>
        /// Events pertaining to autosharded client shard shutdown, clean or otherwise.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 121, Name = "Shard")] 
        ShardShutdown = MaxValue + 121,

        /// <summary>
        /// Events containing raw payloads, as they're received from Discord's REST API.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 122, Name = "Rest")] 
        RestRx = MaxValue + 122,

        /// <summary>
        /// Events containing raw payloads, as they're sent to Discord's REST API.
        /// </summary>
        [LogSource(KnownColor.LimeGreen, MapsToEventId = 123, Name = "Rest")] 
        RestTx = MaxValue + 123,
        
        /// <summary>
        ///     Indicates that this log message came from an unknown source.
        /// </summary>
        [LogSource(KnownColor.Red)]
        Unknown = uint.MaxValue
    }
}