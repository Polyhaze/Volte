using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("CreateConfig")]
        [Summary("Create a config for the guild with the given ID, if one doesn't exist.")]
        [Remarks("Usage: $createconfig [guildId]")]
        public async Task CreateConfig(ulong serverId = 0) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await Context.ReactFailure();
                return;
            }

            if (serverId == 0) serverId = Context.Guild.Id;

            Db.GetConfig(serverId);
            await Reply(Context.Channel, CreateEmbed(Context,
                $"Created a config for **{VolteBot.Client.GetGuild(serverId).Name}** if it didn't exist."));
        }
    }
}