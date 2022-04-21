using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Entities;
using Volte.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Ban")]
        [Description("Bans a member.")]
        public async Task<ActionResult> BanAsync([CheckHierarchy, EnsureNotSelf, Description("The member to ban.")]
            SocketGuildUser member,
            [Remainder, Description("The reason for the ban.")]
            string reason = "Banned by a Moderator.")
        {
            var e = Context
                .CreateEmbedBuilder(
                    $"You've been banned from {Format.Bold(Context.Guild.Name)} for {Format.Bold(reason)}.")
                .ApplyConfig(Context.GuildData);

            if (!await member.TrySendMessageAsync(embed: e.Build()))
                Logger.Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");

            try
            {
                await member.BanAsync(7, reason);
                return Ok($"Successfully banned **{member}** from this guild.", _ =>
                    ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                        .WithDefaultsFromContext(Context)
                        .WithActionType(ModActionType.Ban)
                        .WithTarget(member)
                        .WithReason(reason), Context.CreateEmbedBuilder(), Context.GuildData, Context.Channel)
                );
            }
            catch
            {
                return BadRequest(
                    "An error occurred banning that member. Do I have permission; or are they higher than me in the role list?");
            }
        }

        [Command("UnixBan", "UBan")]
        [Description("Bans the user with custom modifications provided via Unix arguments.")]
        [ShowUnixArgumentsInHelp(VolteUnixCommand.UnixBan)]
        public async Task<ActionResult> UnixBanAsync([CheckHierarchy, EnsureNotSelf, Description("The member to ban.")]
            SocketGuildUser member,
            [Remainder, Description("The modifications to the ban action you'd like to make.")]
            Dictionary<string, string> modifications)
        {
            var daysToDelete = (modifications.TryGetValue("days", out var result) ||
                         modifications.TryGetValue("deleteDays", out result)) &&
                        int.TryParse(result, out var intResult)
                ? intResult
                : 0;

            var reason = modifications.TryGetValue("reason", out result) ? result : "Banned by a Moderator.";

            var e = Context.CreateEmbedBuilder(
                    $"You've been banned from {Format.Bold(Context.Guild.Name)} for {Format.Bold(reason)}.");

            if (!Context.GuildData.Configuration.Moderation.ShowResponsibleModerator ||
                modifications.TryGetValue("shadow", out _))
            {
                e.WithAuthor(author: null).WithSuccessColor();
            }
            
            if (!await member.TrySendMessageAsync(embed: e.Build()))
                Logger.Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");

            try
            {
                await member.BanAsync(daysToDelete, reason);
                if (modifications.TryGetValue("soft", out _) || modifications.TryGetValue("softly", out _))
                    await Context.Guild.RemoveBanAsync(member.Id);
                
                return Ok($"Successfully banned **{member}** from this guild.", _ =>
                    ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                        .WithDefaultsFromContext(Context)
                        .WithActionType(ModActionType.Ban)
                        .WithTarget(member)
                        .WithReason(reason), Context.CreateEmbedBuilder(), Context.GuildData, Context.Channel)
                );
            }
            catch
            {
                return BadRequest(
                    "An error occurred banning that member. Do I have permission; or are they higher than me in the role list?");
            }
        }
    }
}