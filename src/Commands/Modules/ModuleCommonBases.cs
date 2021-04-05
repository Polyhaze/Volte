using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Entities;
using Volte.Core.Helpers;
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
        /// <returns><see cref="UrbanApiResponse"/> if the request was successful; <see langword="null"/> otherwise.</returns>
        public async Task<UrbanApiResponse> GetDefinitionAsync(string word)
        {
            var get = await Http.GetAsync($"https://api.urbandictionary.com/v0/define?term={HttpUtility.UrlEncode(word)}", HttpCompletionOption.ResponseContentRead);


            var toReturn = get.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<UrbanApiResponse>(await get.Content.ReadAsStringAsync())
                : null;

            if (toReturn != null)
                toReturn.Entries = toReturn.Entries.Select(x =>
                {
                    x.Definition = x.Definition.Replace("]", "").Replace("[", "");
                    x.Example = x.Example.Replace("]", "").Replace("[", "");
                    return x;
                }).ToList();

            return toReturn;
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
    public sealed partial class AdminUtilityModule : VolteModule { }

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

            var e = new EmbedBuilder().WithSuccessColor().WithAuthor(issuer)
                .WithDescription($"You've been warned in **{issuer.Guild.Name}** for `{reason}`.");
            if (!data.Configuration.Moderation.ShowResponsibleModerator)
                e.WithAuthor(author: null);

            if (!await member.TrySendMessageAsync(embed: e.Build()))
            {
                Logger.Warn(LogSource.Volte,
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