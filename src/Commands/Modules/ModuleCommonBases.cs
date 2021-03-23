using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Commands.Modules
{
    
    public sealed partial class UtilityModule : VolteModule
    {
        public CommandsService CommandsService { get; set; }
        
        private (IOrderedEnumerable<(string Name, bool Value)> Allowed, IOrderedEnumerable<(string Name, bool Value)> Disallowed) GetPermissions(
            SocketGuildUser user)
        {
            var propDict = user.GuildPermissions.GetType().GetProperties()
                .Where(a => a.PropertyType.Inherits<bool>())
                .Select(a => (a.Name.Humanize(), a.GetValue(user.GuildPermissions).Cast<bool>()))
                .OrderByDescending(ab => ab.Item2 ? 1 : 0)
                .ToList(); //holy reflection

            return (propDict.Where(ab => ab.Item2).OrderBy(a => a.Item1), propDict.Where(ab => !ab.Item2).OrderBy(a => a.Item2));

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
            DatabaseService db, LoggingService logger, string reason)
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

            if (!await member.TrySendMessageAsync(
                embed: e.Build()))
            {
                logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }
        }
    }
    
    [Group("Settings", "Setting", "Options", "Option")]
    [RequireGuildAdmin]
    public partial class SettingsModule : VolteModule
    {
        public WelcomeService WelcomeService { get; set; }

        [Command, DummyCommand]
        public Task<ActionResult> BaseAsync() => None();
    }
}