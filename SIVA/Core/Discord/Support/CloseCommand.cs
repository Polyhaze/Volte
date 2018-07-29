using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Support {
    public class CloseCommand : SivaCommand {
        [Command("Close")]
        public async Task Close() {
            if (Regex.IsMatch(Context.Channel.Name, "^" +
                                                    ServerConfig.Get(Context.Guild).SupportChannelName
                                                    + "-[0-9]{18}$")) {
                await Context.Channel.SendMessageAsync("", false, Utils.CreateEmbed(Context,
                    "Closing ticket in 45 seconds..."));
                await TicketHandler.DeleteTicket(Context.Channel);
            }
            else {
                await Context.Channel.SendMessageAsync("", false,
                    Utils.CreateEmbed(Context, $"**#{Context.Channel.Name}** is not a valid support ticket."));
            }
        }
    }
}