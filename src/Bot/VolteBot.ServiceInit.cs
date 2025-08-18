namespace Volte;

public partial class VolteBot
{
    private const GatewayIntents Intents
        = GatewayIntents.Guilds | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildMembers |
          GatewayIntents.GuildMessages | GatewayIntents.GuildPresences | GatewayIntents.MessageContent | GatewayIntents.GuildBans;

    public static IServiceCollection CreateServiceCollection() =>
        new ServiceCollection()
            .AddSingleton(new HttpClient
            {
                Timeout = 10.Seconds()
            })
            .AddSingleton(new CommandService(new CommandServiceConfiguration
            {
                IgnoresExtraArguments = true,
                StringComparison = StringComparison.OrdinalIgnoreCase,
                DefaultRunMode = RunMode.Sequential,
                SeparatorRequirement = SeparatorRequirement.SeparatorOrWhitespace,
                Separator = " ",
                NullableNouns = null
            }))
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = IsDebugLoggingEnabled
                    ? LogSeverity.Debug
                    : LogSeverity.Verbose,
                GatewayIntents = Intents,
                AlwaysDownloadUsers = true,
                ConnectionTimeout = 10000,
                MessageCacheSize = 50,
                AuditLogCacheSize = 25
            }));


}