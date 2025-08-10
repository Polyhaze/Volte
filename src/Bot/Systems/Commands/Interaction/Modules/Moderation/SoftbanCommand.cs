using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionModerationModule
{
    [SlashCommand("softban", "Kicking a user deleting the last x days of messages.")]
    public async Task<RuntimeResult> SoftBanAsync(
        [Summary("member", "The member to softban."), DoHierarchyCheck]
        SocketGuildUser member,
        [Summary("reason", "The reason for the ban.")]
        string reason,
        [Summary("days_to_delete", "The amount of days of messages to delete."),
            Choice("1", 1),
            Choice("2", 2),
            Choice("3", 3),
            Choice("4", 4),
            Choice("5", 5),
            Choice("6", 6),
            Choice("7", 7)
        ]
        int daysToDelete = 7)
    {
        var e = Context.CreateEmbedBuilder(
            $"You've been softbanned from {Format.Bold(Context.Guild.Name)} for {Format.Bold(reason)}." +
            $"\nA softban simply means you were kicked and a certain amount of your previous messages were mass deleted. " +
            $"You can still join back.");
        
        if (!await member.TrySendMessageAsync(embed: e.Apply(GetData()).Build()))
            Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");
        
        try
        {
            await member.BanAsync(daysToDelete, GetReason(Context.User, reason));
            await Context.Guild.RemoveBanAsync(member.Id);
            
            return Ok($"Successfully softbanned **{member}**.", () => 
                ModService.OnModActionCompleteAsync(ModActionEventArgs
                    .FromModule(this)
                    .WithActionType(ModActionType.Softban)
                    .WithTarget(member)
                    .WithReason(reason)));
        }
        catch
        {
            return BadRequest(
                "An error occurred softbanning that member. Do I have permission; or are they higher than me in the role list?");
        }
    }
}