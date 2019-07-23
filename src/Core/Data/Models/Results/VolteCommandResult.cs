using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Data.Models.Results
{
    public abstract class VolteCommandResult : CommandResult
    {
        public override bool IsSuccessful { get; } = true;

        public abstract Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx);

        public static implicit operator Task<VolteCommandResult>(VolteCommandResult res)
        {
            return Task.FromResult(res);
        }
    }
}