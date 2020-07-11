using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("CreateConfig")]
        [Description("Create a config for the guild with the given ID, if one doesn't exist.")]
        [Remarks("createconfig [Guild]")]
        [RequireBotOwner]
        public Task<ActionResult> CreateConfigAsync([Remainder] SocketGuild guild = null)
        {
            guild ??= Context.Guild;
            return Ok($"Created a config for {Format.Bold(guild.Name)} if it didn't exist.", m =>
            {
                _ = Db.GetData(guild?.Id ?? Context.Guild.Id);
                return Task.CompletedTask;
            });
        }
    }
}