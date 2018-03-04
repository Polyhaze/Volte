using OverwatchAPI;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using FortniteApi;

namespace SIVA.Core.Modules
{
    public class Stats : ModuleBase<SocketCommandContext>
    {
        [Command("OverwatchPlayer"), Alias("OwP")]
        public async Task OverwatchPlayer(string gamer)
        {
            var owClient = new OverwatchClient();
            Player player = await owClient.GetPlayerAsync(gamer);
            var embed = new EmbedBuilder();
            embed.WithTitle($"{gamer}'s Overwatch \"stats\"!");
            embed.AddField("Level", player.PlayerLevel);
            embed.AddField("Platform", player.Platform);
            embed.AddField("Profile URL", player.ProfileUrl);
            embed.AddField("Achievements", player.Achievements.Count);
            embed.WithThumbnailUrl(player.CompetitiveRankImageUrl);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("Fortnite"), Alias("FN")]
        public async Task Fortnite(FortniteApi.Data.Platform platform, [Remainder]string name)
        {
            var fortnite = new FortniteClient("b226f694-eb4a-4e09-99d2-0639bf57ea90");
            var profile = await fortnite.FindPlayerAsync(platform, name);
            var embed = new EmbedBuilder();
            embed.WithTitle($"{name}'s Fortnite Info - {platform}");
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.AddField("Account ID", profile.AccountId);
            embed.AddField("Platform", profile.PlatformName);
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithThumbnailUrl("https://cdn.atr.cloud/monthly_2017_10/FortniteClient-Win64-Shipping_123.ico_256x256.png.9db57869789ecc4d9c5f72c5a9ba9e30.thumb.png.d8d082ccd47b246fc3773e854b1b2ead.png");

            await Context.Channel.SendMessageAsync("", false, embed);
        }

    }
}
