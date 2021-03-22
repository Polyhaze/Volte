using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class SettingsModule
    {
        [Command("Prefix")]
        [Description("Sets the command prefix for this guild.")]
        public Task<ActionResult> PrefixAsync([Remainder] string newPrefix)
        {
            Context.GuildData.Configuration.CommandPrefix = newPrefix;
            Db.Save(Context.GuildData);
            return Ok($"Set this guild's prefix to **{newPrefix}**.");
        }
    }
}