using Discord;
 
using Discord.WebSocket;

namespace Volte.Core.Models.EventArgs
{
    public sealed class ReadyEventArgs : System.EventArgs
    {
        public ReadyEventArgs(DiscordSocketClient client)
        {
            Client = client;
        }

        public DiscordSocketClient Client { get; }
    }
}