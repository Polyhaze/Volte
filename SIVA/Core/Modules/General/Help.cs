using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using SIVA.Core.Bot;

namespace SIVA.Core.Modules.General
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("H")]
        [Summary("Shows useful links for the bot.")]
        public async Task HelpCommand()
        {
            var embed = Helpers.CreateEmbed(Context,
             Bot.Internal.Utilities.GetFormattedLocaleMsg("HelpString", Context.User.Username));

            await Context.Message.AddReactionAsync(new Emoji("☑"));

            var dm = await Context.User.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync("", false, embed);
        }
    }
}
