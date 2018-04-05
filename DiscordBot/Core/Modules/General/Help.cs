using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Modules.General
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("H")]
        [Summary("Shows commands for the bot.")]
        public async Task HelpCommand()
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithDescription(Bot.Utilities.GetFormattedLocaleMsg("HelpString"));
            await Context.Message.AddReactionAsync(new Emoji("☑"));

            var dm = await Context.User.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync("", false, embed);

        }
    }
}
