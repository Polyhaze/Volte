using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("CreateConfig")]
        [Description("Create a config for the guild with the given ID, if one doesn't exist.")]
        [Remarks("createconfig [serverId]")]
        [RequireBotOwner]
        public Task<ActionResult> CreateConfigAsync(ulong serverId = 0)
            => Ok($"Created a config for **{Context.Client.GetGuild(serverId).Name}** if it didn't exist.", m =>
            {
                _ = Db.GetData(serverId is 0 ? Context.Guild.Id : serverId);
                return Task.CompletedTask;
            });
    }
}