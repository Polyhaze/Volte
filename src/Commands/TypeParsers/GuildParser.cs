using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class GuildParser : TypeParser<SocketGuild>
    {
        public override ValueTask<TypeParserResult<SocketGuild>> ParseAsync(
            Parameter parameter,
            string value,
            CommandContext context)
        {
            var ctx = context.AsVolteContext();
            SocketGuild guild = null;

            var guilds = ctx.Client.Guilds;

            if (ulong.TryParse(value, out var id))
                guild = guilds.FirstOrDefault(x => x.Id == id);

            if (guild is null)
            {
                var match = guilds.Where(x =>
                    x.Name.EqualsIgnoreCase(value)).ToArray();
                if (match.Length > 1)
                    return TypeParserResult<SocketGuild>.Unsuccessful(
                        "Multiple guilds found with that name, try using its ID.");

                guild = match.FirstOrDefault();
            }

            return guild is null
                ? TypeParserResult<SocketGuild>.Unsuccessful("Guild not found.")
                : TypeParserResult<SocketGuild>.Successful(guild);
        }
    }
}