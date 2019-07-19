using System.Collections.Generic;
using System.Linq;
using Discord;

namespace Volte.Data.Models.Results
{
    public class ResultCompletionData
    {
        public List<IUserMessage> Messages { get; }

        public ResultCompletionData(params IUserMessage[] messages)
        {
            Messages = messages.ToList();
        }
    }
}