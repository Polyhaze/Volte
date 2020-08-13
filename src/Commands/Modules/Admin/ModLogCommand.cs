using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("ModLog")]
        [Description("Sets the channel to be used for mod log.")]
        [Remarks("modlog {Channel}")]
        public Task<ActionResult> ModLogAsync(SocketTextChannel c)
        {
            ModifyData(data =>
            {
                data.Configuration.Moderation.ModActionLogChannel = c.Id;
                return data;
            });
            return Ok($"Set {c.Mention} as the channel to be used by mod log.");
        }
    }
}