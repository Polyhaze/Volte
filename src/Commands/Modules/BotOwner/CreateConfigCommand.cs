using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("CreateConfig")]
        [Description("Create a config for the guild with the given ID, if one doesn't exist.")]
        [Remarks("Usage: |prefix|createconfig [serverId]")]
        [RequireBotOwner]
        public Task<VolteCommandResult> CreateConfigAsync(ulong serverId = 0)
        {
            if (serverId is 0) serverId = Context.Guild.Id;

            Db.GetData(serverId);
            return Ok($"Created a config for **{Context.Client.GetGuild(serverId).Name}** if it didn't exist.");
        }
    }
}