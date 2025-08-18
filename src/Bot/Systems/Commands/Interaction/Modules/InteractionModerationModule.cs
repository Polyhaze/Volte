namespace Volte.Systems.Commands.Interaction.Modules;

[SlashCommandGroup("mod", "Moderator-only commands.")]
[RequireGuildModeratorPrecondition]
public sealed partial class InteractionModerationModule : VolteSlashCommandModule
{
    public static string GetReason(IUser moderator, string reason) 
        => $"{moderator.Username} ({moderator.Id}): {reason}";
    
    public ModerationService ModService { get; set; }
}