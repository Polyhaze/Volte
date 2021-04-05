using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Core.Helpers;
using Volte.Services;

namespace Volte.Interactive
{
    public class DeleteMessageReactionCallback : IReactionCallback
    {
        public RunMode RunMode { get; } = RunMode.Parallel;
        public ICriterion<SocketReaction> Criterion { get; } = new EnsureReactionFromSourceUserCriterion();
        public TimeSpan? Timeout { get; } = TimeSpan.FromSeconds(10);
        public VolteContext Context { get; }
        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            if (reaction.Emote.Name.EqualsIgnoreCase(DiscordHelper.X))
            {
                return await reaction.Message.Value.TryDeleteAsync();
            }

            return false;

        }

        public DeleteMessageReactionCallback(VolteContext ctx)
        {
            Context = ctx;
        }
    }
}