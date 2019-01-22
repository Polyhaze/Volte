using System.Threading.Tasks;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Modules.General {
    public class PrefixCommand : SIVACommand {
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