using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("CreateConfig")]
        [Description("Create a config for the guild with the given ID, if one doesn't exist.")]
        public Task<ActionResult> CreateConfigAsync([Remainder, Description("The guild to create the config for.")] SocketGuild guild = null)
        {
            guild ??= Context.Guild;
            return Ok($"Created a config for {Format.Bold(guild.Name)} if it didn't exist.", m =>
            {
                _ = Db.GetData(guild.Id);
                return Task.CompletedTask;
            });
        }
    }
}