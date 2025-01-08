using Discord.Interactions;

namespace Volte.Interactions.Commands.Modules;

public sealed partial class InteractionModerationModule
{
    [SlashCommand("ban", "Ban someone from the server.")]
    public async Task<RuntimeResult> BanAsync(
        [Summary("member", "The member to ban."), DoHierarchyCheck]
        SocketGuildUser member,
        [Summary("reason", "The reason for the ban.")]
        string reason)
    {
        var e = Context.CreateEmbedBuilder(
            $"You've been banned from {Format.Bold(Context.Guild.Name)} for {Format.Bold(reason)}.");
        
        if (!await member.TrySendMessageAsync(embed: e.Apply(GetData()).Build()))
            Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");
        
        try
        {
            await member.BanAsync(7, reason);
            
            return Ok($"Successfully banned **{member}** from this guild.", () => 
                ModService.OnModActionCompleteAsync(ModActionEventArgs.FromModule(this)
                    .WithActionType(ModActionType.Ban)
                    .WithTarget(member)
                    .WithReason(reason)));
        }
        catch
        {
            return BadRequest(
                "An error occurred banning that member. Do I have permission; or are they higher than me in the role list?");
        }
    }
}