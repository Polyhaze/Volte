using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Gommon;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Services
{
    public class AddonService : VolteService
    {
        private readonly IServiceProvider _provider;
        private bool _isInitialized;
        public Dictionary<VolteAddonMeta, string> LoadedAddons { get; }
        internal HashSet<ScriptState> AddonResults { get; }

        public AddonService(IServiceProvider serviceProvider)
        {
            _isInitialized = false;
            _provider = serviceProvider;
            LoadedAddons = new Dictionary<VolteAddonMeta, string>();
            AddonResults = new HashSet<ScriptState>();
        }

        public async Task InitAsync()
        {
            var sw = Stopwatch.StartNew();
            if (_isInitialized || !Directory.Exists("addons")) return; //don't auto-create a directory; if someone wants to use addons they need to make it themselves.
            var addonFolders = Directory.GetDirectories("addons");
            if (addonFolders.IsEmpty())
            {
                Logger.Info(LogSource.Service, "No addons are in the addons directory; skipping initialization.");
                return;
            }
            
            foreach (var folder in addonFolders)
            {
                if (TryGetAddonContent(folder, out var meta, out var code))
                    LoadedAddons.Add(meta, code);
                else
                    if (meta != null && code is null)
                        Logger.Error(LogSource.Service, $"Attempted to load addon {meta.Name} but there were no C# source files in its directory. These are necessary as an addon with no logic does nothing.");
            }

            foreach (var (meta, code) in LoadedAddons)
            {
                try
                {
                    AddonResults.Add(await CSharpScript.RunAsync(code, EvalHelper.Options, new AddonEnvironment(_provider)));
                }
                catch (Exception e)
                {
                    Logger.Error(LogSource.Service, $"Addon {meta.Name}'s logic produced an error.", e);
                }
            }
            sw.Stop();
            Logger.Info(LogSource.Service, $"{"addon".ToQuantity(LoadedAddons.Count)} loaded in {sw.Elapsed.Humanize(2)}.");
            _isInitialized = true;

        }

        private bool TryGetAddonContent(string dir, out VolteAddonMeta meta, out string code)
        {
            meta = null;
            code = null;
            foreach (var file in Directory.GetFiles(dir))
            {
                if (file.EndsWith(".json"))
                {
                    try
                    {
                        meta = JsonSerializer.Deserialize<VolteAddonMeta>(File.ReadAllText(file),
                            Config.JsonOptions);
                        if (meta.Name.EqualsIgnoreCase("list"))
                            throw new InvalidOperationException(
                                $"Addon with name {meta.Name} is being ignored because it is using a reserved name. Please change the name or remove the addon.");

                    }
                    catch (JsonException e)
                    {
                        Logger.Error(LogSource.Service, $"Addon meta file '{file}' had invalid JSON contents.", e);
                    }
                    catch (InvalidOperationException e)
                    {
                        meta = null;
                        Logger.Error(LogSource.Service, e.Message);
                    }
                }

                if (file.EndsWith(".cs"))
                    code = File.ReadAllText(file);
            }

            return meta != null && code != null;
        }
    }
}