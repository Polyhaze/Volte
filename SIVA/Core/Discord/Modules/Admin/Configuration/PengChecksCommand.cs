using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration
{
    public class PengChecksCommand : SIVACommand
    {
        [Command("PengChecks")]
        public async Task PengChecks(bool isEnabled)
        {
            if (!UserUtils.IsAdmin(Context))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.MassPengChecks = true;
            ServerConfig.Save();

            var pcIsEnabled = isEnabled ? "Enabled mass ping checks." : "Disabled mass ping checks.";
            await Context.Channel.SendMessageAsync("", false, Utils.CreateEmbed(Context, pcIsEnabled));
        }
    }
}