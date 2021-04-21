using System.Threading.Tasks;
using Volte.Core.Entities;

namespace Volte.Commands
{
    public class NoResult : ActionResult
    {
        public NoResult(AsyncFunction afterCompletion = null, bool awaitCallback = true)
        {
            _after = afterCompletion;
            _awaitCallback = awaitCallback;
        }

        private readonly AsyncFunction _after;
        private readonly bool _awaitCallback;

        public override async ValueTask<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            if (_after is null)
                return new ResultCompletionData();

            if (_awaitCallback)
                await _after();
            else
                _ = _after();
            return new ResultCompletionData();
        }
    }
}