using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace Volte.Core.Data.Models.Results
{
    public class ResultCompletionData
    {
        public List<IUserMessage> Messages { get; }

        public ResultCompletionData(params IUserMessage[] messages)
        {
            Messages = messages.ToList();
        }

        public static implicit operator Task<ResultCompletionData>(ResultCompletionData data)
        {
            return Task.FromResult(data);
        }

        public static implicit operator ValueTask<ResultCompletionData>(ResultCompletionData data)
        {
            return new ValueTask<ResultCompletionData>(data);
        }
    }
}