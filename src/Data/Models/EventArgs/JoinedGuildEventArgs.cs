using Discord;
using Discord.WebSocket;
using Volte.Core;
using Volte.Data.Models.Guild;
using Volte.Services;

namespace Volte.Data.Models.EventArgs
{
    public sealed class JoinedGuildEventArgs
    {
        private DatabaseService _db = DatabaseService.Instance;
        public IGuild Guild { get; }
        public GuildData Data { get; }
        public DiscordSocketClient Client { get; }

        public JoinedGuildEventArgs(SocketGuild guild)
        {
            Guild = guild;
            Data = _db.GetData(guild);
            Client = VolteBot.Client;
        }
    }
}