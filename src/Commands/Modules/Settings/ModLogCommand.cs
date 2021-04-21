using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;

namespace Volte.Commands.Modules
{
    public sealed partial class SettingsModule
    {
        [Command("ModLog")]
        [Description("Sets the channel to be used for mod log.")]
        public Task<ActionResult> ModLogAsync([Description("The channel to be used by my moderation log.")] SocketTextChannel c)
        {
            Context.Modify(data => data.Configuration.Moderation.ModActionLogChannel = c.Id);
            return Ok($"Set {c.Mention} as the channel to be used by mod log.");
        }
    }
}