using Discord;
using Discord.WebSocket;
using DiscordBot;

namespace DiscordBot.Core.LevelingSystem
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
                    embed.WithDescription(Utilities.GetFormattedAlert("LeveledUpMessage", user.Username, newLevel));
                    embed.WithColor(DiscordBot.Config.bot.defaultEmbedColour);
                    embed.WithTitle("Level up!");
                    embed.AddField("XP Amount", userAccount.XP);

                    await channel.SendMessageAsync("", false, embed);
                }
            }
        }
    }
}
