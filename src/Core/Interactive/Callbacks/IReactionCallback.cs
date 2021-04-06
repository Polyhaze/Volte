using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands;

namespace Volte.Interactive
{
    public interface IReactionCallback
    {
        RunMode RunMode { get; }
        ICriterion<SocketReaction> Criterion { get; }
        VolteContext Context { get; }

        Task<bool> HandleAsync(SocketReaction reaction);
    }
}
