using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.General
{
    public class HelpCommand : SIVACommand
    {
        [Command("Help"), Alias("H")]
        public async Task Help()
        {
            var embed = Utils.CreateEmbed(Context,
                "Use [this link](https://discordapp.com/oauth2/authorize?client_id=320942091049893888&scope=bot&permissions=8) to invite the bot into your server.  \n\nJoin our [Support Server](https://discord.gg/prR9Yjq)!  \n\nDeveloper: <@168548441939509248> \n\nFull command [documentation](http://code.greem.xyz/SIVA-Developers/SIVA/wikis/home). \n\n**Commands do not work in DM with the bot.**");

            await Context.Message.AddReactionAsync(new Emoji("☑"));

            await Context.User.SendMessageAsync("", false, embed);
        }
    }
}