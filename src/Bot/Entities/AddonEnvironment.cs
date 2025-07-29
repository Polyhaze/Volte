// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Entities;

public class AddonEnvironment
{
    public AddonEnvironment(IServiceProvider provider)
    {
        Services = provider;
        Client = Services.Get<DiscordSocketClient>();
        Commands = Services.Get<CommandService>();
        Database = Services.Get<DatabaseService>();
    }
    
    public IServiceProvider Services { get; }
    public DiscordSocketClient Client { get; } 
    public CommandService Commands { get; }
    public DatabaseService Database { get; }

    public bool IsCommand(SocketUserMessage message, ulong guildId, out string targetCommand) 
        => CommandUtilities.HasAnyPrefix(message.Content, 
            new[] { Database.GetData(guildId).Configuration.CommandPrefix, $"<@{Client.CurrentUser.Id}> ", $"<@!{Client.CurrentUser.Id}> " }, 
            StringComparison.OrdinalIgnoreCase, out _, out targetCommand);

}