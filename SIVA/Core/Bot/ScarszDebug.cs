using System;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SIVA.Core.Bot.Internal;
using SIVA.Core.JsonFiles;
using SIVA.Core.Modules;

namespace SIVA.Core.Bot
{
    public class ScarszDebug : SivaModule
    {
        private static readonly HttpClientHandler _handler = new HttpClientHandler {AllowAutoRedirect = false};
        private static readonly HttpClient _http = new HttpClient(_handler);
        
        internal static string CreateDebug(string config)
        {
            
            var files = new Dictionary<string, Dictionary<string, string>>
            {
                {"1-Thanks.txt", new Dictionary<string, string> {{"content", "Thanks to Scarsz lol#4227 on Discord for this tool. Without it, we wouldn't be able to do this!\n\nCheck out his stuff here:\nGitHub: https://github.com/Scarsz\nDiscord: https://discord.gg/WdFa6gc\nTwitter: https://twitter.com/ScarszRawr"}}},
                {"2-ServerConfig.txt", new Dictionary<string, string> {{"content", config}}},
                {"3-Info.txt", new Dictionary<string, string> {{"content", "Support Server: https://discord.gg/prR9Yjq\nWebsite: https://greem.xyz\nSource Code: http://code.greem.xyz/SIVA-Developers/SIVA"}}}
            };

            var payload = new
            {
                description = "Discord server settings for support.",
                files
            };
            var payloadJson = JsonConvert.SerializeObject(payload);
            
            var httpClient = new RestClient("https://debug.scarsz.me");
            httpClient.UserAgent = $"SIVA/{Utilities.GetLocaleMsg("VersionString")}";
            var req = new RestRequest("post", Method.POST);
            req.AddHeader("Content-Type", "application/json");
            req.RequestFormat = DataFormat.Json;
            req.Parameters.Clear();
            req.AddParameter("application/json", payloadJson, ParameterType.RequestBody);
            var resJson = httpClient.Execute(req);
            var res = (JObject)JsonConvert.DeserializeObject(resJson.Content);
            return res.GetValue("url").ToString();
                
        }

        [Command("Debug")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CreateDebugToUrl()
        {
            await Helpers.SendMessage(Context, Helpers.CreateEmbed(Context, CreateDebug(CreateConfigString(GuildConfig.GetOrCreateConfig(Context.Guild.Id)))));
        }

        private static string CreateConfigString(Guild config)
        {
            string customCommands = "";
            string blacklistedWords = "";
            string selfRoles = "";
            string antilinkIgnoredChannels = "";
            foreach (KeyValuePair<string, string> command in config.CustomCommands)
            {
                customCommands += $"{command.Key}: {command.Value} || ";
            }

            foreach (string word in config.Blacklist)
            {
                blacklistedWords += $"{word}, ";
            }

            foreach (string role in config.SelfRoles)
            {
                selfRoles += $"{role}, ";
            }

            foreach (ulong channelId in config.AntilinkIgnoredChannels)
            {
                antilinkIgnoredChannels += $"{channelId}, ";
            }
            
            string parsedConfig = $"ServerId: {config.ServerId}\n" +
                                  $"ServerOwner: {config.GuildOwnerId}\n" +
                                  "\n\n" +
                                  "Lists:\n" +
                                  $"    AntilinkIgnoredChannels: {antilinkIgnoredChannels}\n" +
                                  $"    SelfAssignableRoles: {selfRoles}\n" +
                                  $"    Blacklist: {blacklistedWords}\n" +
                                  $"    CustomCommands: {customCommands}\n" +
                                  "\n" +
                                  "Support:\n" +
                                  $"    CanCloseOwnTicket: {config.CanCloseOwnTicket}\n" +
                                  $"    SupportChannelName: {config.SupportChannelName}\n" +
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
                                  $"    WelcomeColorRGB: {config.WelcomeColour1}, {config.WelcomeColour2}, {config.WelcomeColour2}\n" +
                                  $"    WelcomeChannel: {config.WelcomeChannel}\n" +
                                  "\n" +
                                  "Miscellaneous:\n" +
                                  $"    TruthOrDareEnabled: {config.IsTodEnabled}\n" +
                                  $"    IsServerLoggingEnabled: {config.IsServerLoggingEnabled}\n" +
                                  $"    ServerLoggingChannel: {config.ServerLoggingChannel}\n" +
                                  $"    ModeratorRole: {config.ModRole}\n" +
                                  $"    AdminRole: {config.AdminRole}\n" +
                                  $"    CommandPrefix {config.CommandPrefix}\n" +
                                  "\n" +
                                  "Donator:\n" +
                                  $"    CommandEmbedColourRGB: {config.EmbedColour1}, {config.EmbedColour2}, {config.EmbedColour3}\n" +
                                  "    (More coming for donators soon™)";

            return parsedConfig;

        }
    }
}