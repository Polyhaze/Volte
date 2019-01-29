using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.Moderation {
    public partial class ModerationModule : VolteModule {

        [Command("Purge"), Alias("clear", "clean")]
        public async Task Purge(int count) {
            var config = Db.GetConfig(Context.Guild);
            if (!UserUtils.HasRole(Context.User, config.ModRole)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }
            
            await ((ITextChannel) Context.Channel).DeleteMessagesAsync(
                await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, count + 1).FlattenAsync());
        }
        
    }
}