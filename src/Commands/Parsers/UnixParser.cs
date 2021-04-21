using System.Collections.Generic;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Commands
{
    [InjectTypeParser]
    public sealed class UnixParser : VolteTypeParser<Dictionary<string, string>>
    {
        public override ValueTask<TypeParserResult<Dictionary<string, string>>> ParseAsync(string value, VolteContext _) 
            => UnixHelper.TryParseNamedArguments(value, out var result)
            ? Success(result.Parsed)
            : Failure(result.Error.Message);
    }
}