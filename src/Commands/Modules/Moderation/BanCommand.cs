using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands;
using Gommon;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Ban")]
        [Description("Bans a member.")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        public async Task<ActionResult> BanAsync([CheckHierarchy, EnsureNotSelf, Description("The member to ban.")] SocketGuildUser member,
            [Remainder, Description("The reason for the ban.")] string reason = "Banned by a Moderator.")
        {
            var e = Context.CreateEmbedBuilder($"You've been banned from **{Context.Guild.Name}** for **{reason}**.");
            if (!Context.GuildData.Configuration.Moderation.ShowResponsibleModerator)
            {
                e.WithAuthor(author: null);
                e.WithSuccessColor();
            }

            if (!await member.TrySendMessageAsync(embed: e.Build()))
            {
                Logger.Warn(LogSource.Volte, $"encountered a 403 when trying to message {member}!");
            }

            try
            {
                await member.BanAsync(7, reason);
                return Ok($"Successfully banned **{member}** from this guild.", _ =>
                    ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                        .WithDefaultsFromContext(Context)
                        .WithActionType(ModActionType.Ban)
                        .WithTarget(member)
                        .WithReason(reason))
                );
            }
            catch
            {
                return BadRequest("An error occurred banning that member. Do I have permission; or are they higher than me in the role list?");
            }
        }
    }
}