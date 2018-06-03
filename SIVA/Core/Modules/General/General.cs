using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Bot;
using SIVA.Core.Bot.Internal;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Modules.General
{
    public class General : SivaModule
    {
        [Command("Stats")]
        public async Task MyStats([Remainder] string arg = "")
        {
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            var target = mentionedUser ?? Context.User;

            if (Context.Guild.Id == 377879473158356992)
            {
                await ReplyAsync("That command is disabled on this server.");
            }
            else
            {
                var account = UserAccounts.GetAccount(target);
                await ReplyAsync($"**{target.Username}** is level {account.LevelNumber}, and has {account.Xp} XP.");
            }
        }

        [Command("Prefix")]
        public async Task GetPrefixForServer()
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            string prefix;
            switch (config)
            {
                case null:
                    prefix = Config.bot.Prefix;
                    break;
                default:
                    prefix = config.CommandPrefix;
                    break;
            }

            var embed = Helpers.CreateEmbed(Context, $"The prefix for this server is {prefix}.");
            await Helpers.SendMessage(Context, embed);
        }

        [Command("Lenny")]
        public async Task LennyLol()
        {
            var embed = Helpers.CreateEmbed(Context, "( ͡° ͜ʖ ͡°)");

            await Helpers.SendMessage(Context, embed);
        }

        [Command("Say")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task SayCommand([Remainder] string message)
        {
            var embed = Helpers.CreateEmbed(Context, message);
            await Context.Message.DeleteAsync();

            if (Config.bot.Debug)
            {
                var channel = Program._client.GetGuild(405806471578648588).GetTextChannel(431928769465548800);
                await channel.SendMessageAsync("", false,
                    Helpers.CreateEmbed(Context,
                        $"{Context.User.Mention} used the say command in the channel <#{Context.Channel.Id}>, in Guild **{Context.Guild.Name}** and said **\"{message}\"**."));
                await ReplyAsync("", false, embed);
            }
            else
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Command("Choose")]
        public async Task PickOne([Remainder] string message)
        {
            var options = message.Split('|', StringSplitOptions.RemoveEmptyEntries);

            var r = new Random();

            var embed = Helpers.CreateEmbed(Context,
                Bot.Internal.Utilities.GetFormattedLocaleMsg("PickCommandText", options[r.Next(0, options.Length)]));

            await Helpers.SendMessage(Context, embed);
        }

        [Command("Roast")]
        public async Task Roast()
        { //this doesnt have any other roasts as its incomplete
            await ReplyAsync(Context.User.Mention +
                             ", maybe you would talk better if your parents were second cousins rather than first cousins.");
        }

        [Command("Info")]
        public async Task InformationCommand()
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.AddField("Version", Bot.Internal.Utilities.GetLocaleMsg("VersionString"));
            embed.AddField("Author", $"<@{Program._client.CurrentUser.Id}>");
            embed.AddField("Language", "C# with Discord.Net");
            embed.AddField("Server", "https://greem.xyz/SIVA");
            embed.AddField("Servers", Program._client.Guilds.Count);
            embed.AddField("Invite Me", "https://bot.discord.io/SIVA");
            embed.AddField("Ping", Program._client.Latency);
            embed.AddField("Client ID", Program._client.CurrentUser.Id);
            embed.AddField("Invite my Nadeko", "https://bot.discord.io/snadeko");
            embed.WithThumbnailUrl("https://pbs.twimg.com/media/Cx0i4LOVQAIyLRU.png");
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));

            await Helpers.SendMessage(Context, embed);
        }

        [Command("Suggest")]
        public async Task Suggest()
        {
            await ReplyAsync("<https://goo.gl/forms/i6pgYTSnDdMMNLZU2>");
            await Helpers.SendMessage(Context, msg: "https://goo.gl/forms/i6pgYTSnDdMMNLZU2");
        }
    }
}