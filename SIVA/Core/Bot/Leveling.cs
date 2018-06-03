using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Bot.Internal;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Bot
{
    internal static class Leveling
    {
        internal static async Task UserSentMessage(SocketGuildUser user, SocketTextChannel channel)
        {
            var userAccount = UserAccounts.GetAccount(user);
            var oldLevel = userAccount.LevelNumber;
            userAccount.Xp += 5;
            userAccount.Money += 1;
            UserAccounts.SaveAccounts();
            var newLevel = userAccount.LevelNumber;

            if (oldLevel != newLevel)
            {
                var embed = new EmbedBuilder();
                embed.WithDescription(Utilities.GetFormattedLocaleMsg("LeveledUpMessage", user.Username, newLevel));
                embed.WithColor(Config.bot.DefaultEmbedColour);
                embed.WithTitle("Level up!");
                embed.WithDescription($"Good job **{user.Mention}**! You leveled up to level **{newLevel}**!");

                var lvlUpMsg = await channel.SendMessageAsync("", false, embed);
                Thread.Sleep(5000);
                await lvlUpMsg.DeleteAsync();
            }
        }
    }
}