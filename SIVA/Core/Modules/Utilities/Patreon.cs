using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Bot.Internal;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Modules.Utilities
{
    public class Patreon : ModuleBase<SocketCommandContext>
    {
        private readonly string patreonLink = "https://patreon.com/_SIVA";

        [Command("Donate")]
        public async Task SendDonateEmbed()
        {
            var embed = new EmbedBuilder()
                .WithDescription($"Donate to my Patreon!\n\n{patreonLink}")
                .WithColor(Config.bot.DefaultEmbedColour)
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            await ReplyAsync("", false, embed);
        }

        [Command("VerifyCheck")]
        [Alias("Vc")]
        public async Task CheckIfServerIsVerified()
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithDescription(config.VerifiedGuild
                ? "This server is verified! Greem thanks you for your generosity. (Unless this is a server Greem owns)"
                : $"This server isn't verified. Donate on my Patreon to unlock cool perks! (Plus you can brag to your friends about it.)\n\n{patreonLink}");

            await ReplyAsync("", false, embed);
        }

        [Command("Donators")]
        public async Task ListDonatorMentions()
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithDescription(
                    $"Nobody has donated yet! Maybe you'll be the first? Use `{config.CommandPrefix}donate` to get to there.")
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await ReplyAsync("", false, embed);
        }
    }
}