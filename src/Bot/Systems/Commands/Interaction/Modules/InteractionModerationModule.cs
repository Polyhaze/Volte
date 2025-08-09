using Volte.Systems.Moderation;

namespace Volte.Systems.Commands.Interaction.Modules;

[Discord.Interactions.Group("mod", "Moderator-only commands.")]
[RequireGuildModeratorPrecondition]
public sealed partial class InteractionModerationModule : VolteSlashCommandModule
{
    public ModerationService ModService { get; set; }
}