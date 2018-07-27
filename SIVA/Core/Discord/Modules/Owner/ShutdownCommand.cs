using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Owner {
    public class ShutdownCommand : SIVACommand {
        // I'm not sure how well this works.

        [Command("Shutdown")]
        public async Task Shutdown() {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await Context.Message.AddReactionAsync(new Emoji(new RawEmoji().X));
                return;
            }

            await SIVA.GetInstance().LogoutAsync();
            await SIVA.GetInstance().StopAsync();
            Environment.Exit(0);
        }
    }
}