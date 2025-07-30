using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;


namespace Volte.Systems.CSharpScripting;

public sealed class AddonService : VolteService
{
    public static readonly FilePath AddonsDir = new("addons", true);
        
    private readonly IServiceProvider _provider;
    private bool _isInitialized;
    public Dictionary<VolteAddon, ScriptState> LoadedAddons { get; }

    public AddonService(IServiceProvider serviceProvider)
    {
        _isInitialized = false;
        _provider = serviceProvider;
        LoadedAddons = new Dictionary<VolteAddon, ScriptState>();
    }
    
    public async Task InitAsync()
    {
        var sw = Stopwatch.StartNew();
        if (_isInitialized || !AddonsDir.ExistsAsDirectory) 
            return; //don't auto-create a directory; if someone wants to use addons they need to make it themselves.
        if (AddonsDir.GetSubdirectories().Count < 1)
        {
            Info(LogSource.Service, "No addons are in the addons directory; skipping initialization.");
            return;
        }

        foreach (var addon in GetAvailableAddons())
        {
            try
            {
                LoadedAddons.Add(addon, await CSharpScript.RunAsync(addon.Script, EvalHelper.Options, new AddonEnvironment(_provider)));
            }
            catch (Exception e)
            {
                Error(LogSource.Service, $"Addon {addon.Meta.Name}'s script produced an error.", e);
            }
        }
        sw.Stop();
        Info(LogSource.Service, $"{"addon".ToQuantity(LoadedAddons.Count)} loaded in {sw.Elapsed.Humanize(2)}.");
        _isInitialized = true;
    }
    
    private static IEnumerable<VolteAddon> GetAvailableAddons()
    {
        foreach (var dir in AddonsDir.GetSubdirectories())
        {
            if (TryGetAddonContent(dir, out var addon))
                yield return addon;
                
            if (addon.Meta != null && addon.Script is null)
                Error(LogSource.Service,
                    $"Attempted to load addon {addon.Meta.Name} but there were no C# source files in its directory. These are necessary as an addon with no logic does nothing.");
        }
    }

    private static bool TryGetAddonContent(FilePath addonDir, out VolteAddon addon)
    {
        addon = new();
            
        foreach (var file in addonDir.GetFiles())
        {
            if (file.Extension is "json")
            {
                try
                {
                    addon.Meta = JsonSerializer.Deserialize<VolteAddonMeta>(file.ReadAllText(), Config.JsonOptions);
                        
                    if (addon.Meta.Name.EqualsIgnoreCase("list"))
                        throw new InvalidOperationException(
                            $"Addon with name {addon.Meta.Name} is being ignored because it is using a reserved name. Please change the name or remove the addon.");

                }
                catch (JsonException e)
                {
                    Error(LogSource.Service, $"Addon meta file '{file}' had invalid JSON contents.", e);
                }
                catch (InvalidOperationException e)
                {
                    addon.Meta = null;
                    Error(LogSource.Service, e.Message);
                }
            }

            if (file.Extension is "cs")
                addon.Script = file.ReadAllText();
        }

        return addon.Meta != null && addon.Script != null;
    }
}