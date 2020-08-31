namespace Volte.Core.Entities
{
    public enum LogSource : uint
    {
        /// <summary>
        ///     Indicates that this log message is from a command or module.
        /// </summary>
        Module = 1,
        /// <summary>
        ///     Indicates that this log message is from a Service.
        /// </summary>
        Service = 2,
        /// <summary>
        ///     Indicates that this log message is from Discord.Net.
        /// </summary>
        Discord = 3,
        /// <summary>
        ///     Indicates that this log message is from the Discord HTTP REST API.
        /// </summary>
        Rest = 4,
        /// <summary>
        ///     Indicates that this log message is from the Discord WebSocket Gateway connection.
        /// </summary>
        Gateway = 5,
        /// <summary>
        ///     Indicates that this log message is from the Volte itself.
        /// </summary>
        Volte = 6,
        
        Interactivity = 7,
        
        AutoShard = 8,

        WebSocket = 9,
        
        WebSocketDispatch = 10,
        
        DSharpPlus = 11,


        /// <summary>
        ///     Indicates that this log message came from an unknown source.
        /// </summary>
        Unknown = uint.MaxValue
    }
}