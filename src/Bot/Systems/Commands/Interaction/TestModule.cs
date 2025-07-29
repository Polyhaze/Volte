#if DEBUG

using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction;

public class TestModule : VolteSlashCommandModule
{
    [SlashCommand("test", "slash command api test")]
    public Task<RuntimeResult> TestAsync(string parameter1)
    {
        return None();
    }
}

#endif