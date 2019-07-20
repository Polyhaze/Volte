using System.Threading.Tasks;
using Volte.Commands;

namespace Volte.Data.Models.Results
{
    public class NoResult : VolteCommandResult
    {
        public override bool IsSuccessful => true;

        public override Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            return Task.FromResult(new ResultCompletionData());
        }
    }
}