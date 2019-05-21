using Discord.WebSocket;
using Qmmands;
using Volte.Commands;
using Volte.Commands.TypeParsers;
using Volte.Core;
using Volte.Services;

namespace Volte.Extensions
{
    public static class CommandExtensions
    {
        public static string SanitizeName(this Module m)
            => m.Name.Replace("Module", string.Empty);

        public static string SanitizeRemarks(this Command c, VolteContext ctx)
        {
            var db = VolteBot.GetRequiredService<DatabaseService>();
            var aliases = $"({string.Join("|", c.FullAliases)})";
            return (c.Remarks ?? "No usage provided")
                .Replace(c.Name.ToLower(), (c.FullAliases.Count > 1 ? aliases : c.Name).ToLower())
                .Replace("|prefix|", db.GetData(ctx.Guild).Configuration.CommandPrefix)
                .Replace("Usage: ", string.Empty);
        }

        internal static void AddTypeParsers(this CommandService service)
        {
            service.AddTypeParser(new UserParser<SocketGuildUser>());
            service.AddTypeParser(new UserParser<SocketUser>());
            service.AddTypeParser(new RoleParser<SocketRole>());
            service.AddTypeParser(new ChannelParser<SocketTextChannel>());
            service.AddTypeParser(new EmoteParser());
            service.AddTypeParser(new GuildParser());
            service.AddTypeParser(new BooleanParser(), true);
        }
    }
}