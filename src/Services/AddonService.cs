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

        public AddonService(IServiceProvider serviceProvider)
        {
            _isInitialized = false;
            _provider = serviceProvider;
            LoadedAddons = new Dictionary<VolteAddonMeta, string>();
        }

        public async Task InitAsync()
        {
            var sw = Stopwatch.StartNew();
            if (!Directory.Exists("addons") || _isInitialized) return; //don't auto-create a directory; if someone wants to use addons they need to make it themselves.
            var addonFolders = Directory.GetDirectories("addons");
            if (addonFolders.IsEmpty())
            {
                Logger.Info(LogSource.Service, "No addons are in the addons directory; skipping initialization.");
                return;
            }
            
            foreach (var folder in addonFolders)
            {
                VolteAddonMeta meta = null;
                string code = null;
                foreach (var file in Directory.GetFiles(folder))
                {
                    if (file.EndsWith(".json"))
                    {
                        try
                        {
                            meta = JsonSerializer.Deserialize<VolteAddonMeta>(await File.ReadAllTextAsync(file),
                                Config.JsonOptions);
                            if (meta.Name.EqualsIgnoreCase("list"))
                            {
                                throw new InvalidOperationException(
                                    $"Addon with name {meta.Name} is being ignored because it is using a reserved name. Please change the name or remove it.");
                            }
                            
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
                        code = await File.ReadAllTextAsync(file);
                }

                if (meta != null && code is null)
                    Logger.Error(LogSource.Service, $"Attempted to load addon {meta.Name} but there were no .cs files in its directory. These are necessary as an addon with no logic does nothing.");

                if (meta != null && code != null)
                    LoadedAddons.Add(meta, code);

            }

            foreach (var (meta, code) in LoadedAddons)
            {
                try
                {
                    if ((await CSharpScript.RunAsync(code, EvalHelper.Options, new AddonEnvironment(_provider))).ReturnValue != null)
                        Logger.Info(LogSource.Service, "Addon's logic resulted in a value; ignoring. This could lead to garbage collection issues, be wary!");
                }
                catch (Exception e)
                {
                    Logger.Error(LogSource.Service, $"Addon {meta.Name}'s logic produced an error.", e);
                }

            }
            sw.Stop();
            Logger.Info(LogSource.Service, $"{"addon".ToQuantity(LoadedAddons.Count)} loaded in {sw.Elapsed.Humanize()}.");
            _isInitialized = true;

        }
    }
}