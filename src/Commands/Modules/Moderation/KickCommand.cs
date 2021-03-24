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
        [Command("Kick")]
        [Description("Kicks the given user.")]
        [RequireBotGuildPermission(GuildPermission.KickMembers)]
        public async Task<ActionResult> KickAsync([CheckHierarchy, EnsureNotSelf, Description("The member to kick.")] SocketGuildUser user,
            [Remainder, Description("The reason for the kick.")] string reason = "Kicked by a Moderator.")
        {
            var e = Context.CreateEmbedBuilder($"You've been kicked from **{Context.Guild.Name}** for **{reason}**.");
            if (!Context.GuildData.Configuration.Moderation.ShowResponsibleModerator)
                e.WithAuthor(author: null);
            
            if (!await user.TrySendMessageAsync(embed: e.Build()))
            {
                Logger.Warn(LogSource.Volte, $"encountered a 403 when trying to message {user}!");
            }
            
            try
            {
                await user.KickAsync(reason);
                return Ok($"Successfully kicked **{user.Username}#{user.Discriminator}** from this guild.", _ =>
                    ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                        .WithDefaultsFromContext(Context)
                        .WithActionType(ModActionType.Kick)
                        .WithTarget(user)
                        .WithReason(reason))
                );
            }
            catch
            {
                return BadRequest("An error occurred kicking that user. Do I have permission; or are they higher than me in the role list?");
            }
        }
    }
}