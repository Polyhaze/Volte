using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Volte.Services
{
    public class CacheService : VolteEventService
    {
        public readonly Dictionary<ulong, DiscordPresence> CachedPresences;

        public CacheService()
        {
            CachedPresences = new Dictionary<ulong, DiscordPresence>();
        }

        public override Task DoAsync(EventArgs args)
        {
            return args switch
            {
                PresenceUpdateEventArgs presenceUpdated => HandlePresenceUpdated(presenceUpdated),
                _ => Task.CompletedTask
            };
        }

        public DiscordPresence GetBotPresence(DiscordShardedClient client)
        {
            return CachedPresences.First(x => x.Key == client.CurrentUser.Id).Value;
        }

        private async Task HandlePresenceUpdated(PresenceUpdateEventArgs args)
        {
            await Task.Yield();
            if (args.User.Id == args.Client.CurrentUser.Id)
            {
                CachedPresences.Clear();
                CachedPresences.Add(args.User.Id, args.PresenceAfter); 
            }
        }
    }
}