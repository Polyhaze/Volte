using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Moderation {
    public class PurgeCommand : SIVACommand {

        [Command("purge"), Alias("clear", "clean", "")]
        public async Task Purge(int count) {
            var config = ServerConfig.Get(Context.Guild);
            if (!UserUtils.HasRole((SocketGuildUser)Context.User, Context.Guild.Roles.FirstOrDefault(r => r.Id == config.ModRole))) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var toPurge = new List<ulong>();
            Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, count)
                .ForEach(m => toPurge.Add(m.First().Id));
            await ((ITextChannel) Context.Channel).DeleteMessagesAsync(toPurge);
        }
        
    }
}