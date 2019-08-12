using System.Threading.Tasks;
using Discord;

namespace Volte.Commands.Results
{
    public class ResultCompletionData
    {
        public IUserMessage Message { get; }

        public ResultCompletionData(IUserMessage message) 
            => Message = message;

        public ResultCompletionData() 
            => Message = null;

        public static implicit operator ValueTask<ResultCompletionData>(ResultCompletionData data) 
            => new ValueTask<ResultCompletionData>(data);
    }
}