using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Owner
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
                .CreateEmbed($"Created a config for **{VolteBot.Client.GetGuild(serverId).Name}** if it didn't exist.")
                .SendTo(Context.Channel);
        }
    }
}