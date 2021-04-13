using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("CreateConfig")]
        [Description("Create a config for the guild, if one doesn't exist.")]
        public Task<ActionResult> CreateConfigAsync(
            [Remainder,
             Description(
                 "The guild to create the config for. If none is provided, it will make one for the current guild.")]
            SocketGuild guild = null)
        {
            guild ??= Context.Guild;
            return Ok($"Created a config for {Format.Bold(guild.Name)} if it didn't exist.", async m =>
            {
                await Task.Yield();
                _ = Db.GetData(guild.Id);
            });
        }
    }
}