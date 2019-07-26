using System;
using System.Threading.Tasks;
using Volte.Commands;

namespace Volte.Commands.Results
{
    public class NoResult : ActionResult
    {
        public NoResult(Func<Task> afterCompletion = null)
            => _after = afterCompletion;

        private readonly Func<Task> _after;

        public override async Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            if (_after is null)
            {
                return new ResultCompletionData();
            }

            await _after();
            return new ResultCompletionData();
        }
    }
}