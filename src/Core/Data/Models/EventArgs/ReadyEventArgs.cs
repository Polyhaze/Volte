using Discord.WebSocket;

namespace Volte.Core.Data.Models.EventArgs
{
    public sealed class ReadyEventArgs : System.EventArgs
    {
        public DiscordSocketClient Client { get; }

        public ReadyEventArgs(DiscordSocketClient client)
        {
            Client = client;
        }
    }
}