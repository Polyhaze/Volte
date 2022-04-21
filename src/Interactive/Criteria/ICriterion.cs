using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Volte.Interactive
{
    public interface ICriterion<in T>
    {
        ValueTask<bool> CheckAsync(SocketUserMessage message, T parameter);
    }
}
