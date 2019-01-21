using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using SIVA.Core.Files.Readers;
using SIVA.Core.Files.Objects;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules {
    public class DebugCommand : SIVACommand {
        private static string Execute(string config) {
            var files = new Dictionary<string, Dictionary<string, string>> {
                {
                    "1-Info.txt", new Dictionary<string, string> {
                        {
                            "content",
                            "Thanks to Scarsz lol#4227 on Discord for this tool. Without it, we wouldn't be able to do this!\n\nCheck out his stuff here:\nWebsite: https://scarsz.me\nGitHub: https://github.com/Scarsz\nDiscord: https://discord.gg/WdFa6gc\nTwitter: https://twitter.com/ScarszRawr\n\nSupport Server: https://discord.gg/prR9Yjq\nWebsite: https://greem.xyz\nSource Code: https://github.com/Greeem/SIVA"
                        }, {
                            "description", "Thanks Scarsz for this amazing utility!"
                        }
                    }
                }, {
                    "2-Server.conf", new Dictionary<string, string> {
                        {
                            "content", $"{config}"
                        }, {
                            "description", "Server config for debug purposes."
                        }
                    }
                }
            };

            var payload = new {
                description = "Discord server settings for support.", files
            };

            var httpClient = new RestClient("https://debug.scarsz.me") { UserAgent = "SIVA/V2" };
            var req = new RestRequest("post", Method.POST);
            req.AddHeader("Content-Type", "application/json");
            req.RequestFormat = DataFormat.Json;
            req.Parameters.Clear();
            req.AddParameter("application/json", JsonConvert.SerializeObject(payload), ParameterType.RequestBody);
            var resJson = httpClient.Execute(req);
            return ((JObject)JsonConvert
                .DeserializeObject(resJson.Content))
                .GetValue("url").ToString()
                .Replace("scarsz.me", "greem.me");
        }

        [Command("ForceDebug")]
        [RequireOwner]
        public async Task ForceDebug(ulong serverId) {
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context,
                    CreateConfigString(ServerConfig.Get(SIVA.Client.GetGuild(serverId)))
                )
            );
        }

        [Command("Debug")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Debug() {
            await Context.Channel.SendMessageAsync("",
                false,
                CreateEmbed(Context,
                    $"{Execute(CreateConfigString(ServerConfig.Get(Context.Guild)))}" +
                    "\n\nTake this to the SIVA server for support. Join the server [here](https://greem.xyz/SIVA)."));
        }

        private static string CreateConfigString(Server config) {
            var customCommands = "";
            var blacklistedWords = "";
            var selfRoles = "";
            var antilinkIgnoredChannels = "";
            foreach (var command in config.CustomCommands) customCommands += $"{command.Key}: {command.Value} || ";

            foreach (var word in config.Blacklist) blacklistedWords += $"{word}, ";

            foreach (var role in config.SelfRoles) selfRoles += $"{role}, ";

            foreach (var channelId in config.AntilinkIgnoredChannels) antilinkIgnoredChannels += $"{channelId}, ";

            var parsedConfig = $"ServerId: {config.ServerId}\n" +
                               $"ServerOwner: {config.GuildOwnerId}\n" +
                               "\n\n" +
                               "Lists:\n" +
                               $"    AntilinkIgnoredChannels: {antilinkIgnoredChannels}\n" +
                               $"    SelfAssignableRoles: {selfRoles}\n" +
                               $"    Blacklist: {blacklistedWords}\n" +
                               $"    CustomCommands: {customCommands}\n" +
                               "\n" +
                               "Support:\n" +
                               $"    SupportChannelName: {config.SupportChannelName}\n" +
                               $"    SupportCategoryId: {config.SupportCategoryId}" +
                               $"    SupportChannelId: {config.SupportChannelId}\n" +
                               $"    SupportRole: {config.SupportRole}\n" +
                               "\n" +
                               "Moderation:\n" +
                               $"    AntiLink: {config.Antilink}\n" +
                               $"    MassPingChecks: {config.MassPengChecks}\n" +
                               "\n" +
                               "Welcome/Leaving:\n" +
                               $"    WelcomeMessage: {config.WelcomeMessage}\n" +
                               $"    LeavingMessage: {config.LeavingMessage}\n" +
                               $"    WelcomeColorRGB: {config.WelcomeColourR}, {config.WelcomeColourG}, {config.WelcomeColourB}\n" +
                               $"    WelcomeChannel: {config.WelcomeChannel}\n" +
                               "\n" +
                               "Miscellaneous:\n" +
                               $"    ModeratorRole: {config.ModRole}\n" +
                               $"    AdminRole: {config.AdminRole}\n" +
                               $"    CommandPrefix {config.CommandPrefix}\n" +
                               "\n" +
                               "Donator:\n" +
                               $"    CommandEmbedColourRGB: {config.EmbedColourR}, {config.EmbedColourG}, {config.EmbedColourB}\n" +
                               "    (More coming for donators soon™)";

            return parsedConfig;
        }
    }
}