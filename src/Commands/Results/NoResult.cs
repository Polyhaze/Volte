using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Volte.Commands.Results
{
    public class NoResult : ActionResult
    {
        private readonly Func<Task> _after;

        public NoResult(Func<Task> afterCompletion = null)
        {
            _after = afterCompletion;
        }

        public override Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            if (_after is null) return new ResultCompletionData();

            _ = _after();
            return new ResultCompletionData();
        }
    }
}