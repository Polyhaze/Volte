using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Volte.Commands.Results
{
    public class ResultCompletionData
    {
        public ResultCompletionData(IUserMessage message)
        {
            Message = message;
        }

        public ResultCompletionData()
        {
            Message = null;
        }

        public IUserMessage Message { get; }

        public static implicit operator Task<ResultCompletionData>(ResultCompletionData data)
        {
            return Task.FromResult(data);
        }
    }
}