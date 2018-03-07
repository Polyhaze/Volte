using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using SIVA.Core.Config;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace SIVA.Core.Modules
{
    public class General : ModuleBase<SocketCommandContext>
    {
        [Command("Stats")]
        public async Task MyStats([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = mentionedUser ?? Context.User;
            
            if (Context.Guild.Id == 377879473158356992)
            {
                await ReplyAsync("That command is disabled on this server.");
            }
            else
            {
                var account = UserAccounts.UserAccounts.GetAccount(target);
                await ReplyAsync($"**{target.Username}** is level {account.LevelNumber}, and has {account.XP} XP and {account.Points} points.");
            }
        }

        [Command("Prefix")]
        public async Task GetPrefixForServer()
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            var prefix = "$";
            switch (config)
            {
                case null:
                    prefix = SIVA.Config.bot.Prefix;
                    break;
                default:
                    prefix = config.CommandPrefix;
                    break;
            }

            embed.WithDescription($"The prefix for this server is `{prefix}`");
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            await ReplyAsync("", false, embed);
        }

        [Command("Lenny")]
        public async Task LennyLol()
        {
            await ReplyAsync("( ͡° ͜ʖ ͡°)");
        }

        /*public class CatReply
        {
            public String status { get; set;}
            public String message { get; set;}
        }

        [Command("Cat")]
        public async Task RandomCat()
        {
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString("https://random.cat/meow");
            }

            var ReplyObject = JsonConvert.DeserializeObject<CatReply>(json);
            var status = ReplyObject.status;
            var catImageUrl = ReplyObject.message;
            var embed = new EmbedBuilder();
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithImageUrl(catImageUrl);
            await ReplyAsync("", false, embed);
        }*/

        [Command("Say")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task SayCommand([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(message);
            embed.WithColor(new Color(SIVA.Config.bot.DefaultEmbedColour));
            await Context.Message.DeleteAsync();


            if (SIVA.Config.bot.Debug)
            {
                Console.WriteLine($"DEBUG: {Context.User.Username}#{Context.User.Discriminator} used the say command in the channel #{Context.Channel.Name} and said \"{Context.Message.Content}\".");
                await ReplyAsync("", false, embed);
            } 
            else
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Choose")]
        public async Task PickOne([Remainder]string message)
        {
            string[] options = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            Random r = new Random();
            string selection = options[r.Next(0, options.Length)];

            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("PickCommandText", selection));
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);

            await ReplyAsync("", false, embed);
        }

        [Command("Roast")]
        public async Task Roast()
        {   //this doesnt have any other roasts as its incomplete
            await ReplyAsync(Context.User.Mention + ", maybe you would talk better if your parents were second cousins rather than first cousins.");
        }

        [Command("Info")]
        public async Task InformationCommand()
        {
            var Embed = new EmbedBuilder();
            Embed.AddField("Version", "1.1.0");
            Embed.AddField("Author", "<@168548441939509248>");
            Embed.AddField("Language", "C# with Discord.Net");
            Embed.AddField("Server", "https://discord.io/SIVA");
            Embed.AddField("Servers", (Context.Client as DiscordSocketClient).Guilds.Count);
            Embed.AddField("Invite Me", "https://bot.discord.io/SIVA");
            Embed.AddField("Ping", (Context.Client as DiscordSocketClient).Latency);
            Embed.AddField("Client ID", "410547925597421571");
            Embed.AddField("Invite my Nadeko", "https://bot.discord.io/snadeko");
            Embed.WithThumbnailUrl("https://pbs.twimg.com/media/Cx0i4LOVQAIyLRU.png");
            Embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            Embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);

            await ReplyAsync("", false, Embed);

        }

        [Command("Suggest")]
        public async Task Suggest()
        {
            await ReplyAsync("https://goo.gl/forms/i6pgYTSnDdMMNLZU2");
        }

    }
}
