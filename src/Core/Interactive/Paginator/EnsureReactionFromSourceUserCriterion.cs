using System.Threading.Tasks;
using Discord.WebSocket;
using Volte.Commands;

namespace Volte.Interactive
{
    internal class EnsureReactionFromSourceUserCriterion : ICriterion<SocketReaction>
    {
        public Task<bool> JudgeAsync(VolteContext sourceContext, SocketReaction parameter) 
            => Task.FromResult(parameter.UserId == sourceContext.User.Id);
    }
}
