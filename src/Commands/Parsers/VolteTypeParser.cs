using System.Threading.Tasks;
using Gommon;
using Qmmands;

namespace Volte.Commands
{
    public abstract class VolteTypeParser<T> : TypeParser<T>
    {
        public abstract ValueTask<TypeParserResult<T>> ParseAsync(Parameter parameter, string value, VolteContext context);

        public override ValueTask<TypeParserResult<T>> ParseAsync(Parameter parameter, string value,
            CommandContext context) => ParseAsync(parameter, value, context.Cast<VolteContext>());
    }
}
