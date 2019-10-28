using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule : VolteModule
    {
        [Command("ServerName", "Sn", "GuildName", "Gn")]
        [Description("Sets the name of the current guild.")]
        [Remarks("servername {name}")]
        [RequireBotGuildPermission(GuildPermission.ManageGuild)]
        [RequireGuildAdmin]
        public async Task<ActionResult> ServerNameAsync([Remainder] string name)
        {
            await Context.Guild.ModifyAsync(g => g.Name = name);
            return Ok($"Set this guild's name to **{name}**!");
        }
    }
}