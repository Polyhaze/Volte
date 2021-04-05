using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Commands;

namespace Volte.Interactive
{
    internal class EnsureIsIntegerCriterion : ICriterion<SocketMessage>
    { 
        public Task<bool> JudgeAsync(VolteContext sourceContext, SocketMessage parameter)
        {
            var ok = int.TryParse(parameter.Content, out _);
            return Task.FromResult(ok);
        }
    }
}
