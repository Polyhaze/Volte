using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Volte.Commands.Results
{
    public class ResultCompletionData
    {
        public DiscordMessage Message { get; }

        public ResultCompletionData(DiscordMessage message) 
            => Message = message;

        public ResultCompletionData() 
            => Message = null;

        public static implicit operator ValueTask<ResultCompletionData>(ResultCompletionData data) 
            => new ValueTask<ResultCompletionData>(data);
    }
}