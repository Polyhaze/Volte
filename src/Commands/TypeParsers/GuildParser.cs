using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    public sealed class GuildParser : TypeParser<SocketGuild>
    {
        public override Task<TypeParserResult<SocketGuild>> ParseAsync(
            Parameter parameter,
            string value,
            ICommandContext context,
            IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            var guild = default(SocketGuild);

            var guilds = ctx.Client.Guilds;

            if (ulong.TryParse(value, out var id))
                guild = guilds.FirstOrDefault(x => x.Id == id);

            if (guild is null)
            {
                var match = guilds.Where(x =>
                    x.Name.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1)
                    return Task.FromResult(TypeParserResult<SocketGuild>.Unsuccessful(
                        "Multiple guilds found, try or using its ID."));

                guild = match.FirstOrDefault();
            }

            return Task.FromResult(guild is null
                ? TypeParserResult<SocketGuild>.Unsuccessful("Guild not found.")
                : TypeParserResult<SocketGuild>.Successful(guild));
        }
    }
}