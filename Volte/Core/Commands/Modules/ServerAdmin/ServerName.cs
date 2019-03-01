using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;

namespace Volte.Core.Commands.Modules.ServerAdmin
{
    public partial class ServerAdminModule : VolteModule
    {
        [Command("ServerName", "Sn")]
        [Description("Sets the name of the current server.")]
        [Remarks("Usage: |prefix|servername {name}")]
        [RequireBotGuildPermission(GuildPermission.ManageGuild | GuildPermission.Administrator)]
        [RequireGuildAdmin]
        public async Task ServerNameAsync([Remainder] string name)
        {
            await Context.Guild.ModifyAsync(g => g.Name = name);
            await Context.ReactSuccessAsync();
        }
    }
}