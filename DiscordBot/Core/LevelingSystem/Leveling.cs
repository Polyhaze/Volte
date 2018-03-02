using Discord;
using Discord.WebSocket;
using SIVA;

namespace SIVA.Core.LevelingSystem
{
    internal static class Leveling
    {
        internal static async void UserSentMessage(SocketGuildUser user, SocketTextChannel channel)
        {
            if (channel.Guild.Id != 377879473158356992) {

                var userAccount = UserAccounts.UserAccounts.GetAccount(user);
                uint oldLevel = userAccount.LevelNumber;
                userAccount.XP += 5;
                userAccount.Money += 1;
                UserAccounts.UserAccounts.SaveAccounts();
                uint newLevel = userAccount.LevelNumber;

                if (oldLevel != newLevel)
                {
                    var embed = new EmbedBuilder();
                    embed.WithDescription(Utilities.GetFormattedLocaleMsg("LeveledUpMessage", user.Username, newLevel));
                    embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
                    embed.WithTitle("Level up!");
                    embed.AddField("XP Amount", userAccount.XP);

                    await channel.SendMessageAsync("", false, embed);
                }
            }
        }
    }
}
