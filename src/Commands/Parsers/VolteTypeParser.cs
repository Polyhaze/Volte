using System.Threading.Tasks;
using Gommon;
using Qmmands;

namespace Volte.Commands
{
    public abstract class VolteTypeParser<T> : TypeParser<T>
    {
        public abstract ValueTask<TypeParserResult<T>> ParseAsync(string value, VolteContext ctx);

        public override ValueTask<TypeParserResult<T>> ParseAsync(Parameter _, string value,
            CommandContext context) => ParseAsync(value, context.Cast<VolteContext>());
    }
}
