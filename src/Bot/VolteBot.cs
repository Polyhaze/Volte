using Volte.Systems.Commands.Text.Modules;

namespace Volte;

public class VolteBot
{
    public const bool IsProduction =
#if PROD
        true;
#else
        false;
#endif
    
    public static Task StartAsync(Gommon.Optional<CancellationTokenSource> cts = default)
    {
        Console.Title = $"Volte {Version.InformationVersion}";
        Console.CursorVisible = false;
        return LoginAsync(cts);
    }

    public static bool IsHeadless { get; set; }

    public static ServiceProvider Services { get; private set; }

    public static DiscordSocketClient Client { get; private set; }
    public static CancellationTokenSource Cts { get; private set; }

    public VolteBot()
        => Console.CancelKeyPress += (_, _) => Cts?.Cancel();

    public static async Task LoginAsync(Gommon.Optional<CancellationTokenSource> cts = default)
    {
        if (!Config.StartupChecks<HeadlessBotConfig>()) return;

        Config.Load<HeadlessBotConfig>();

        if (!Config.IsValidToken()) return;

        LogFileRestartNotice();

        Services = new ServiceCollection().AddAllServices()
            .AddSingleton(cts.OrElse(new CancellationTokenSource()))
            .BuildServiceProvider();

        Client = Services.Get<DiscordSocketClient>();
        Cts = Services.Get<CancellationTokenSource>();

        AdminUtilityModule.AllowedPasteSites = await HttpHelper.GetAllowedPasteSitesAsync(Services);

        SetAppStatus("Logging in", FontAwesome.RightToBracket);
        var sw = Stopwatch.StartNew();
        await Client.LoginAsync(TokenType.Bot, Config.Token);
        await Client.StartAsync();

        {
            var commandService = Services.Get<CommandService>();

            var addedParsers = commandService.AddTypeParsers();
            Info(LogSource.Volte,
                $"Loaded TypeParsers: [{
                    addedParsers.Select(x => x.Name.Replace("Parser", string.Empty)).JoinToString(", ")
                }]");

            var addedModules = commandService.AddModules(Assembly.GetExecutingAssembly());
            Info(LogSource.Volte,
                $"Loaded {addedModules.Count} modules and {addedModules.Sum(m => m.Commands.Count)} commands.");
        }

        Client.RegisterVolteEventHandlers(Services);

        ExecuteBackgroundAsync(async () => await Services.Get<AddonService>().InitAsync());
        Services.Get<ReminderService>().Initialize();

        try
        {
            SetAppStatus($"Logged in, took {sw.Elapsed.Humanize(2)}.",
                FontAwesome.Check,
                isWorkingStatus: false,
                statusExpiresAfter: 10.Seconds()
            );
            await Task.Delay(-1, Cts.Token);
        }
        catch (Exception e)
        {
            e.SentryCapture();

            await ShutdownAsync();
        }
    }

    // ReSharper disable SuggestBaseTypeForParameter
    public static async Task ShutdownAsync()
    {
        Critical(LogSource.Volte, "Bot shutdown requested; shutting down and cleaning up.");

        CalledCommandsInfo.UpdateSaved(Services.Get<MessageService>());

        await Client.SetStatusAsync(UserStatus.Invisible);
        await Client.LogoutAsync();
        await Client.StopAsync();

        await Services.DisposeAsync();

        Services = null;
        Client = null;
        Cts = null;
    }
}