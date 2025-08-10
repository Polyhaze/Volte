using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionModerationModule
{
    [SlashCommand("kick", "Kick someone from the server.")]
    public async Task<RuntimeResult> KickAsync(
        [Summary("member", "The member to kick."), DoHierarchyCheck]
        SocketGuildUser member,
        [Summary("reason", "The reason for the kick.")]
        string reason)
    {
        var e = Context.CreateEmbedBuilder(
            $"You've been kicked from {Format.Bold(Context.Guild.Name)} for {Format.Bold(reason)}.");
        
        if (!await member.TrySendMessageAsync(embed: e.Apply(GetData()).Build()))
            Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");
        
        try
        {
            await member.KickAsync(GetReason(Context.User, reason));
            
            return Ok($"Successfully kicked **{member}** from this guild.", ModAction
                .WithActionType(ModActionType.Kick)
                .WithTarget(member)
                .WithReason(reason)
            );
        }
        catch
        {
            return BadRequest(
                "An error occurred kicking that member. Do I have permission; or are they higher than me in the role list?");
        }
    }
}