namespace Volte.Systems.Commands.Interaction.Modules;

[SlashCommandGroup("settings", "View & modify settings in your guild. Admin only.")]
[RequireGuildAdminPrecondition]
public partial class InteractionSettingsModule : VolteSlashCommandModule
{
    [SlashCommandGroup("welcome", "View & modify settings related to the welcome system. Admin only.")]
    public partial class WelcomeModule : VolteSlashCommandModule;
}