using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class GuildParser : TypeParser<DiscordGuild>
    {
        public override ValueTask<TypeParserResult<DiscordGuild>> ParseAsync(
            Parameter parameter,
            string value,
            CommandContext context)
        {
            var ctx = context.AsVolteContext();
            DiscordGuild guild = null;

            var isId = ulong.TryParse(value, out var id);
            if (isId)
            {
                foreach (var clientGuilds in ctx.Client.ShardClients.Select(e => e.Value.Guilds))
                {
                    if (clientGuilds.TryGetValue(id, out guild))
                    {
                        break;
                    }
                }
            }

            if (guild is null)
            {
                // SLOW AS HELL
                var match = ctx.Client.ShardClients
                    .SelectMany(e => e.Value.Guilds)
                    .Where(e => e.Value.Name.EqualsIgnoreCase(value)).ToArray();

                if (match.Length > 1)
                    return TypeParserResult<DiscordGuild>.Unsuccessful(
                        "Multiple guilds found with that name, try using its ID.");

                guild = match.FirstOrDefault().Value;
            }

            return guild is null
                ? TypeParserResult<DiscordGuild>.Unsuccessful("Guild not found.")
                : TypeParserResult<DiscordGuild>.Successful(guild);
        }
    }
}