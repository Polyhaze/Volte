using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands;
using Volte.Commands.TypeParsers;
using Volte.Discord;
using Volte.Services;

namespace Volte.Extensions
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
            service.AddTypeParser(new UserParser<DiscordMember>());
            service.AddTypeParser(new UserParser<DiscordUser>());
            service.AddTypeParser(new RoleParser<DiscordRole>());
            service.AddTypeParser(new ChannelParser<DiscordChannel>());
            service.AddTypeParser(new EmoteParser<DiscordGuildEmoji>());
            service.AddTypeParser(new EmoteParser<DiscordEmoji>());
            service.AddTypeParser(new BooleanParser(), true);
        }
    }
}