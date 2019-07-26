using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;

namespace Volte.Commands.Results
{
    public class ResultCompletionData
    {
        public IUserMessage Message { get; }

        public ResultCompletionData(IUserMessage message) 
            => Message = message;

        public ResultCompletionData() 
            => Message = null;

        public static implicit operator Task<ResultCompletionData>(ResultCompletionData data) 
            => Task.FromResult(data);
    }
}