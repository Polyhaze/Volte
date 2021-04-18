using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Entities;
using Volte.Core.Helpers;
using Volte.Interactive;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        public CommandsService CommandsService { get; set; }
        public HttpClient Http { get; set; }

        /// <summary>
        ///     Sends an HTTP <see cref="HttpMethod.Get"/> request to Urban Dictionary's public API requesting the definitions of <paramref name="word"/>.
        /// </summary>
        /// <param name="word">The word/phrase to search for. This method URL encodes it.</param>
        /// <returns><see cref="IEnumerable{UrbanEntry}"/> if the request was successful; <see langword="null"/> otherwise.</returns>
        public async Task<UrbanEntry[]> RequestUrbanDefinitionsAsync(string word)
        {
            var get = await Http.GetAsync(
                $"https://api.urbandictionary.com/v0/define?term={HttpUtility.UrlEncode(word)}".Trim(),
                HttpCompletionOption.ResponseContentRead);


            var apiResp = get.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<UrbanApiResponse>(await get.Content.ReadAsStringAsync())
                : null;

            return apiResp != null
                ? apiResp.Entries.ToArray()
                : Array.Empty<UrbanEntry>();
        }

        private (IOrderedEnumerable<(string Name, bool Value)> Allowed, IOrderedEnumerable<(string Name, bool Value)>
            Disallowed) GetPermissions(
                IGuildUser user)
        {
            var propDict = user.GuildPermissions.GetType().GetProperties()
                .Where(a => a.PropertyType.Inherits<bool>())
                .Select(a => (a.Name.Humanize(LetterCasing.Title), a.GetValue(user.GuildPermissions).Cast<bool>()))
                .OrderByDescending(ab => ab.Item2 ? 1 : 0)
                .ToList(); //holy reflection

            return (propDict.Where(ab => ab.Item2).OrderBy(a => a.Item1),
                propDict.Where(ab => !ab.Item2).OrderBy(a => a.Item2));
        }
    }

    [RequireGuildAdmin]
    public sealed partial class AdminUtilityModule : VolteModule
    {
        public static Dictionary<string[], string> AnnounceNamedArguments = new Dictionary<string[], string>()
        {
            {new[] {"crosspost", "publish"}, "Whether or not to automatically publish this message if it's sent in an announcement channel. This option takes no value."},
            {new[] {"ping", "mention"}, "none, everyone, here, or a role ID"},
            {new[] {"foot", "footer"}, "Set the embed's footer content."},
            {new[] {"thumbnail"}, "Set the embed's small thumbnail URL."},
            {new[] {"image"}, "Set the embed's large image URL."},
            {new[] {"desc", "description"}, "Set the embed's description content."},
            {new[] {"title"}, "Set the embed's title content."},
            {new[] {"color"}, "Set the embed's color."},
            {new[] {"author"}, "Set the author of the embed. `self` or `me` will make you the author; `bot`, `you`, or `volte` will make volte the author, or you can use a server member's ID."}
        };
    }

    [RequireBotOwner]
    public sealed partial class BotOwnerModule : VolteModule
    {
        public HttpClient Http { get; set; }
        public CancellationTokenSource Cts { get; set; }
        public AddonService Addon { get; set; }
    }

    [RequireGuildModerator]
    public sealed partial class ModerationModule : VolteModule
    {
        public InteractiveService Interactive { get; set; }

        public static async Task WarnAsync(SocketGuildUser issuer, GuildData data, SocketGuildUser member,
            DatabaseService db, string reason)
        {
            data.Extras.AddWarn(w =>
            {
                w.User = member.Id;
                w.Reason = reason;
                w.Issuer = issuer.Id;
                w.Date = DateTimeOffset.Now;
            });
            db.Save(data);

            var e = new EmbedBuilder().WithRelevantColor(issuer).WithAuthor(issuer)
                .WithDescription($"You've been warned in {Format.Bold(issuer.Guild.Name)} for {Format.Code(reason)}.")
                .ApplyConfig(data);

            if (!await member.TrySendMessageAsync(embed: e.Build()))
            {
                Logger.Warn(LogSource.Module,
                    $"encountered a 403 when trying to message {member}!");
            }
        }
    }

    [Group("Settings", "Setting", "Options", "Option")]
    [Description("The set of commands used to modify how Volte functions in your guild.")]
    [RequireGuildAdmin]
    public partial class SettingsModule : VolteModule
    {
        public WelcomeService WelcomeService { get; set; }

        [Command, DummyCommand, Description("The set of commands used to modify how Volte functions in your guild.")]
        public Task<ActionResult> BaseAsync() => None();
    }
}