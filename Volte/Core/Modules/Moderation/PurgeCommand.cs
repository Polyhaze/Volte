using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Moderation {
    public partial class ModerationModule : VolteModule {

        [Command("Purge"), Alias("clear", "clean")]
        public async Task Purge(int count) {
            var config = Db.GetConfig(Context.Guild);
            if (!UserUtils.HasRole((SocketGuildUser)Context.User, Context.Guild.Roles.FirstOrDefault(r => r.Id == config.ModRole))) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var toPurge = new List<ulong>();
            Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, count + 1)
                .ForEach(m => toPurge.Add(m.First().Id));
            await ((ITextChannel) Context.Channel).DeleteMessagesAsync(toPurge);
        }
        
    }
}