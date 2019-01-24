using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.General {
    public class PrefixCommand : VolteCommand {
        [Command("Prefix")]
        public async Task Prefix() {
            await Context.Channel.SendMessageAsync(
                string.Empty,
                false,
                CreateEmbed(
                    Context,
                    $"The prefix for this server is `{ServerConfig.Get(Context.Guild).CommandPrefix}`."
                )
            );
        }
    }
}