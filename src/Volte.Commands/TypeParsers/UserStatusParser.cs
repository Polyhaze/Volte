using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class UserStatusParser : TypeParser<UserStatus>
    {
        public override ValueTask<TypeParserResult<UserStatus>> ParseAsync(
            Parameter _, 
            string value, 
            CommandContext __)
        {
            if (value.EqualsIgnoreCase("Online"))
                return TypeParserResult<UserStatus>.Successful(UserStatus.Online);
            if (value.EqualsIgnoreCase("Idle"))
                return TypeParserResult<UserStatus>.Successful(UserStatus.Idle);
            if (value.EqualsIgnoreCase("Dnd"))
                return TypeParserResult<UserStatus>.Successful(UserStatus.DoNotDisturb);
            if (value.EqualsIgnoreCase("Invisible"))
                return TypeParserResult<UserStatus>.Successful(UserStatus.Invisible);
            return TypeParserResult<UserStatus>.Unsuccessful(
                "Available options for this type are `online`, `idle`, `dnd`, or `invisible`.");
        }
    }
}