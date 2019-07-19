using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;

namespace Volte.Data.Models.Results
{
    public abstract class BaseResult : CommandResult
    {
        public override bool IsSuccessful { get; }

        public abstract Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx);

        public static implicit operator Task<BaseResult>(BaseResult res)
        {
            return Task.FromResult(res);
        }

        public static implicit operator ValueTask<BaseResult>(BaseResult res)
        {
            return new ValueTask<BaseResult>(res);
        }
    }
}