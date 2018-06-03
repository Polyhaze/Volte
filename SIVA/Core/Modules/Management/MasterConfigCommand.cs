using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Bot;
using SIVA.Core.Bot.Internal;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Modules.Management
{
    public class MasterConfigCommand : SivaModule
    {
        private DiscordSocketClient _client = Program._client;

        public static string ConvertBoolean(bool boolean)
        {
            return boolean ? "**On**" : "**Off**";
        }

        public static string ConvertList(List<string> list, int count)
        {
            return list.Count >= count ? "**On**" : "**Off**";
        }

        public static string ConvertList(List<ulong> list, int count)
        {
            return list.Count >= count ? "**On**" : "**Off**";
        }

        public static string ConvertDict(Dictionary<string, string> dict, int count)
        {
            return dict.Count >= count ? "**On**" : "**Off**";
        }


        [Command("Config")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task MasterConfig()
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = Helpers.CreateEmbed(Context, $"Server ID: {config.ServerId}\n" +
                                                     $"Owner: <@{config.GuildOwnerId}>");

            var modRole = "**Not set**";
            var adminRole = "**Not set**";
            if (config.ModRole != 0)
                modRole = $"**{Context.Guild.Roles.First(role => role.Id == config.ModRole).Name}**";
            else if (config.AdminRole != 0)
                adminRole = $"**{Context.Guild.Roles.First(role => role.Id == config.ModRole).Name}**";


            if (config.WelcomeChannel != 0)
                embed.AddField("Welcome/Leaving", "On:\n" +
                                                  $"- Channel: <#{config.WelcomeChannel}>\n" +
                                                  $"- WelcomeMsg: {config.WelcomeMessage}\n" +
                                                  $"- Colours: {config.WelcomeColour1} {config.WelcomeColour2} {config.WelcomeColour3}\n" +
                                                  $"- LeavingMsg: {config.LeavingMessage}");
            else
                embed.AddField("Welcome/Leaving", "Off");

            if (config.SupportChannelId != 0)
                embed.AddField("Support", "On\n" +
                                          $"- Channel: <#{config.SupportChannelId}>\n" +
                                          $"- Role: {config.SupportRole}\n" +
                                          $"- Channel Name: {config.SupportChannelName}\n" +
                                          $"- Close Own Ticket: {config.CanCloseOwnTicket}\n");
            else
                embed.AddField("Support", "Off");

            embed.AddField("Other", $"Antilink: {ConvertBoolean(config.Antilink)}\n" +
                                    $"Mass Ping Checks: {ConvertBoolean(config.MassPengChecks)}\n" +
                                    $"Blacklist: {ConvertList(config.Blacklist, 1)}\n" +
                                    $"Custom Commands: {ConvertDict(config.CustomCommands, 1)}\n" +
                                    $"Autorole: {config.Autorole}\n" +
                                    $"Leveling: {config.Leveling}\n" +
                                    $"Server Logging: {ConvertBoolean(config.IsServerLoggingEnabled)}\n" +
                                    $"Mod Role: {modRole}\n" +
                                    $"Admin Role: {adminRole}\n" +
                                    $"Truth or Dare: {ConvertBoolean(config.IsTodEnabled)}\n");

            embed.WithThumbnailUrl(Context.Guild.IconUrl);

            await Helpers.SendMessage(Context, embed);
        }
    }
}