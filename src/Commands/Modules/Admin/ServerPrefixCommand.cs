using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("ServerPrefix")]
        [Description("Sets the command prefix for this guild.")]
        [Remarks("Usage: |prefix|serverprefix {newPrefix}")]
        [RequireGuildAdmin]
        public Task<ActionResult> ServerPrefixAsync([Remainder] string newPrefix)
        {
            Context.GuildData.Configuration.CommandPrefix = newPrefix;
            Db.UpdateData(Context.GuildData);
            return Ok($"Set this server's prefix to **{newPrefix}**.");
        }
    }
}