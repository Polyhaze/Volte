using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class AdminUtilityModule : VolteModule
    {
        [Command("ServerName", "Sn")]
        [Description("Sets the name of the current server.")]
        [Remarks("Usage: |prefix|servername {name}")]
        [RequireBotGuildPermission(GuildPermission.ManageGuild)]
        [RequireGuildAdmin]
        public async Task<VolteCommandResult> ServerNameAsync([Remainder] string name)
        {
            await Context.Guild.ModifyAsync(g => g.Name = name);
            return Ok($"Set this server's name to **{name}**!");
        }
    }
}