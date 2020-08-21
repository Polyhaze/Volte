using System.Threading.Tasks;
using Discord.WebSocket;
using Volte.Commands;
using Volte.Core;

namespace Volte.Interactive
{
    public class EnsureReactionFromBotOwnerCriterion : ICriterion<SocketReaction>
    {
        public Task<bool> JudgeAsync(VolteContext ctx, SocketReaction rxn)
        {
            return Task.FromResult(rxn.UserId == Config.Owner);
        }
    }
}