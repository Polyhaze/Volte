using Discord;
using Discord.WebSocket;
using Volte.Discord;
using Volte.Services;

namespace Volte.Data.Objects.EventArgs
{
    public sealed class ReactionAddedEventArgs
    {
        private DatabaseService _db = VolteBot.GetRequiredService<DatabaseService>();
        public IUserMessage Message { get; }
        public ISocketMessageChannel Channel { get; }
        public IGuild Guild { get; }
        public SocketReaction Reaction { get; }
        public DiscordServer Config { get; }
        public DiscordSocketClient Client { get; }

        public ReactionAddedEventArgs(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            Message = message.Value;
            Channel = channel;
            Reaction = reaction;
            Guild = (channel as ITextChannel)?.Guild;
            Config = _db.GetConfig(Guild);
            Client = VolteBot.Client;
        }
    }
}