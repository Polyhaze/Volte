using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Commands;

namespace Volte.Interactive
{
    public class EmptyCriterion<TParameter> : ICriterion<TParameter>
    {
        public ValueTask<bool> CheckAsync(SocketUserMessage _, TParameter __)
            => new ValueTask<bool>(true);
    }
}
