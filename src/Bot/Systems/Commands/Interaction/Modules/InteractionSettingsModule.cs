namespace Volte.Systems.Commands.Interaction.Modules;

[Discord.Interactions.Group("settings", "View & modify settings in your guild. Admin only.")]
[RequireGuildAdmin]
public partial class InteractionSettingsModule : VolteSlashCommandModule
{
    [Discord.Interactions.Group("welcome", "View & modify settings related to the welcome system. Admin only.")]
    public partial class WelcomeModule : VolteSlashCommandModule;
}