using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;

namespace Volte.Data.Models.Results
{
    public abstract class VolteCommandResult : CommandResult
    {
        public override bool IsSuccessful { get; }

        public abstract Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx);

        public static implicit operator Task<VolteCommandResult>(VolteCommandResult res)
        {
            return Task.FromResult(res);
        }

        public static implicit operator ValueTask<VolteCommandResult>(VolteCommandResult res)
        {
            return new ValueTask<VolteCommandResult>(res);
        }
    }
}