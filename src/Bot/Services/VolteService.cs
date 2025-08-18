using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Volte.Services;

/// <summary>
///     Base for every Volte service, discoverable by the RegisterEventHandlers extension method.
/// </summary>
public abstract class VolteService
{
    private static bool IsEligibleService(Type type) => type.Inherits<VolteService>() && !type.IsAbstract;

    public static void AddOneInstanceOfEachSubtype(IServiceCollection serviceCollection)
    {
        //get all the classes that inherit VolteService, and aren't abstract; add them to the service collection
        var l = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(IsEligibleService)
            .Apply(ls => ls.ForEach(serviceCollection.TryAddSingleton));
        Info(LogSource.Volte, $"Injected services [{l.Select(static x => x.Name.ReplaceIgnoreCase("Service", string.Empty)).JoinToString(", ")}] into the provider.");
    }
}