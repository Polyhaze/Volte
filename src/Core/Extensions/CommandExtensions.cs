using Discord.WebSocket;
using Qmmands;
using Volte.Core.Commands;
using Volte.Core.Commands.TypeParsers;
using Volte.Core.Discord;
using Volte.Core.Services;

namespace Volte.Core.Extensions
{
    public static class CommandExtensions
    {
        public static string SanitizeName(this Module m) => 
            m.Name.Replace("Module", string.Empty);

        public static string SanitizeRemarks(this Command c, VolteContext ctx)
        {
            var db = VolteBot.GetRequiredService<DatabaseService>();
            var aliases = $"({string.Join("|", c.FullAliases)})";
            return (c.Remarks ?? "No usage provided")
                .Replace(c.Name.ToLower(), (c.FullAliases.Count > 1 ? aliases : c.Name).ToLower())
                .Replace("|prefix|", db.GetConfig(ctx.Guild).CommandPrefix)
                .Replace("Usage: ", string.Empty);
        }

        internal static void AddTypeParsers(this CommandService service)
        {
            service.AddTypeParser(new UserParser<SocketGuildUser>());
            service.AddTypeParser(new RoleParser<SocketRole>());
            service.AddTypeParser(new ChannelParser<SocketTextChannel>());
            service.AddTypeParser(new EmoteParser());
            service.AddTypeParser(new BooleanParser(), true);
        }
    }
}