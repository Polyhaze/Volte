using System;
using System.Threading.Tasks;

namespace Volte.Commands.Results
{
    public class NoResult : ActionResult
    {
        public NoResult(Func<Task> afterCompletion = null, bool awaitCallback = true)
        {
            _after = afterCompletion;
            _awaitCallback = awaitCallback;
        }

        private readonly Func<Task> _after;
        private readonly bool _awaitCallback;

        public override async ValueTask<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            if (_after is null)
            {
                return new ResultCompletionData();
            }

            if (_awaitCallback)
                await _after();
            else
                _ = _after();
            return new ResultCompletionData();
        }
    }
}