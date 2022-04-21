using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Entities;

namespace Volte.Commands
{
    [InjectTypeParser]
    public sealed class GuildParser : VolteTypeParser<SocketGuild>
    {
        public override ValueTask<TypeParserResult<SocketGuild>> ParseAsync(string value, VolteContext ctx)
        {
            SocketGuild guild = default;

            var guilds = ctx.Client.Guilds;

            if (ulong.TryParse(value, out var id))
                guild = guilds.FirstOrDefault(x => x.Id == id);

            if (guild is null)
            {
                var match = guilds.Where(x =>
                    x.Name.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1)
                    return Failure(
                        "Multiple guilds found with that name, try using its ID.");

                guild = match.FirstOrDefault();
            }

            return guild is null
                ? Failure("Guild not found.")
                : Success(guild);
        }
    }
}