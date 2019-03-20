using Discord.WebSocket;

namespace Volte.Data.Objects.EventArgs
{
    public class ReadyEventArgs
    {
        public DiscordSocketClient Client { get; }

        public ReadyEventArgs(DiscordSocketClient client)
        {
            Client = client;
        }
    }
}