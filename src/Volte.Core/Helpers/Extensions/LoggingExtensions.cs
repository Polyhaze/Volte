using Microsoft.Extensions.Logging;
using Volte.Core.Entities;

namespace Gommon
{
    public static partial class Extensions
    {
        public static LogSource GetSource(this EventId id)
        {
            if (id.Name.ContainsIgnoreCase("Interactivity")) return LogSource.Interactivity;
            if (id.Name.ContainsIgnoreCase("DSharpPlus")) return LogSource.DSharpPlus;
            if (id.Name.ContainsIgnoreCase("WebSocket")) return LogSource.WebSocket;
            if (id.Name.ContainsIgnoreCase("REST")) return LogSource.Rest;
            if (id.Name.ContainsIgnoreCase("Gateway")) return LogSource.Gateway;

            return LogSource.Unknown;
        }
    }
}