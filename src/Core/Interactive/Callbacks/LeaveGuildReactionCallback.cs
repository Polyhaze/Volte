using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Core.Helpers;

namespace Volte.Interactive
{
    // ReSharper disable UnassignedGetOnlyAutoProperty
    public class LeaveGuildReactionCallback : IReactionCallback
    {
        public RunMode RunMode { get; } = RunMode.Parallel;
        public ICriterion<SocketReaction> Criterion { get; } = new EnsureReactionFromBotOwnerCriterion();
        public TimeSpan? Timeout { get; } = TimeSpan.FromMinutes(10);
        public VolteContext Context { get;} //not used, so it's never initialized, but it has to be here because we implement IReactionCallback.
        public SocketGuild Guild { get; }

        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            if (reaction.Emote.Name.EqualsIgnoreCase(EmojiHelper.X))
            {
                await Guild.LeaveAsync();
                return true;
            }

            return false;

        }

        public LeaveGuildReactionCallback(SocketGuild guild)
        {
            Guild = guild;
        }
    }
}