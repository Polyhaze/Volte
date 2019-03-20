using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        [Command("CreateConfig")]
        [Description("Create a config for the guild with the given ID, if one doesn't exist.")]
        [Remarks("Usage: |prefix|createconfig [serverId]")]
        [RequireBotOwner]
        public async Task CreateConfigAsync(ulong serverId = 0)
        {
            if (serverId == 0) serverId = Context.Guild.Id;

            Db.GetConfig(serverId);
            await Context
                .CreateEmbed($"Created a config for **{Context.Client.GetGuild(serverId).Name}** if it didn't exist.")
                .SendToAsync(Context.Channel);
        }
    }
}