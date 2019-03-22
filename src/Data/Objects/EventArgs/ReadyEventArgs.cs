using Discord.WebSocket;

namespace Volte.Data.Objects.EventArgs
{
    public sealed class ReadyEventArgs
    {
        public DiscordSocketClient Client { get; }

        public ReadyEventArgs(DiscordSocketClient client)
        {
            Client = client;
        }
    }
}