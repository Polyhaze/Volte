using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public partial class ModerationModule
    {
        [Command("Verify", "V")]
        [Description("Verifies a member.")]
        public async Task<ActionResult> VerifyAsync([Remainder, EnsureNotSelf, Description("The user to verify; by removing the Unverified role and granting the Verified role.")] SocketGuildUser member)
        {
            var e = Context.CreateEmbedBuilder($"You've been verified in **{Context.Guild.Name}**.");
            if (!Context.GuildData.Configuration.Moderation.ShowResponsibleModerator)
                e.WithAuthor(author: null);
            if (!await member.TrySendMessageAsync(embed: e.Build()))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }
            
            var uRole = Context.Guild.GetRole(Context.GuildData.Configuration.Moderation.UnverifiedRole);
            var vRole = Context.Guild.GetRole(Context.GuildData.Configuration.Moderation.VerifiedRole);
            if (uRole is null)
                return BadRequest("This guild does not have an Unverified role set. Please use the VerifyRole command.");
            if (vRole is null)
                return BadRequest("This guild does not have a Verified role set. Please use the VerifyRole command.");
            if (member.HasRole(vRole.Id))
                return BadRequest("Member is already verified.");
            if (!member.HasRole(uRole.Id))
                return BadRequest($"Member does not have the Unverified role ({uRole.Mention}).");

            await member.RemoveRoleAsync(uRole);
            await member.AddRoleAsync(vRole);
            
            return Ok($"Successfully verified {member.Mention}.", async _ =>
            {
                await ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Verify)
                    .WithTarget(member));
            });
        }
    }
}