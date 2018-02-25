using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIVA
{
    public static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static ulong MessageIdToTrack { get; set; }
    }
}
