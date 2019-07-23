using System.Threading.Tasks;
using Volte.Commands;

namespace Volte.Core.Data.Models.Results
{
    public class NoResult : VolteCommandResult
    {
        public override Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            return new ResultCompletionData();
        }
    }
}