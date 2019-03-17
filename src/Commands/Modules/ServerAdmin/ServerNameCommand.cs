using System.Threading.Tasks;
using DSharpPlus;
using Qmmands;
using Volte.Commands.Preconditions;

namespace Volte.Commands.Modules.ServerAdmin
{
    public partial class ServerAdminModule : VolteModule
    {
        [Command("ServerName", "Sn")]
        [Description("Sets the name of the current server.")]
        [Remarks("Usage: |prefix|servername {name}")]
        [RequireBotGuildPermission(Permissions.ManageGuild, Permissions.Administrator)]
        [RequireGuildAdmin]
        public async Task ServerNameAsync([Remainder] string name)
        {
            await Context.Guild.ModifyAsync(g => g.Name = name);
            await Context.ReactSuccessAsync();
        }
    }
}