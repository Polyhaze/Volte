using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Core.Commands;
using Volte.Core.Discord;
using Volte.Core.Services;

namespace Volte.Core.Extensions
{
    public static class ModuleExtensions
    {
        public static string SanitizeName(this Module m)
        {
            return m.Name.Replace("Module", string.Empty);
        }

        public static string SanitizeRemarks(this Command c, VolteContext ctx)
        {
            var db = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>();
            var aliases = $"({string.Join("|", c.FullAliases)})";
            return (c.Remarks ?? "No usage provided")
                .Replace(c.Name.ToLower(), aliases)
                .Replace("|prefix|", db.GetConfig(ctx.Guild).CommandPrefix)
                .Replace("Usage: ", string.Empty);
        }
    }
}