using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Commands;

namespace Volte.Interactive
{
    public interface ICriterion<in T>
    {
        ValueTask<bool> JudgeAsync(SocketUserMessage message, T parameter);
    }
}
