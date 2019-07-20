using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules
{
    public partial class AdminModule
    {
        [Command("ModLog")]
        [Description("Sets the channel to be used for mod log.")]
        [Remarks("Usage: |prefix|modlog {channel}")]
        [RequireGuildAdmin]
        public Task<VolteCommandResult> ModLogAsync(SocketTextChannel c)
        {
            var data = Db.GetData(Context.Guild);
            data.Configuration.Moderation.ModActionLogChannel = c.Id;
            Db.UpdateData(data);
            return Ok($"Set {c.Mention} as the channel to be used by mod log.");
        }
    }
}