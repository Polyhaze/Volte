using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace SIVA.Core.Discord.Modules.Admin.Configuration {
    public class WelcomeCommands : SIVACommand {
        [Command("WelcomeChannel"), Alias("Wc")]
        public async Task WelcomeChannel(SocketTextChannel channel) {
            throw new NotImplementedException();
        }

        [Command("WelcomeMessage"), Alias("Wmsg")]
        public async Task WelcomeMessage([Remainder] string message = "") {
            throw new NotImplementedException();
        }

        [Command("WelcomeColour"), Alias("WelcomeColor", "Wcl")]
        public async Task WelcomeColour(int r, int g, int b) {
            throw new NotImplementedException();
        }

        [Command("LeavingMessage"), Alias("Lmsg")]
        public async Task LeavingMessage() {
            throw new NotImplementedException();
        }
    }
}