using System.Threading.Tasks;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Information.Patreon {
    public class DonatorsCommand : SIVACommand {
        [Command("Donators")]
        public async Task Donators() {
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context,
                    "No one has donated yet. Sad face. If you want to be the first one, feel free to donate over at " +
                    $"`{ServerConfig.Get(Context.Guild).CommandPrefix}donate`!"));
        }
    }
}