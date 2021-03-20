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
        private readonly LoggingService _logger;
        private readonly IServiceProvider _provider;
        public Dictionary<VolteAddonInfo, string> LoadedAddons { get; }

        public AddonService(LoggingService loggingService,
            IServiceProvider serviceProvider)
        {
            _logger = loggingService;
            _provider = serviceProvider;
            LoadedAddons = new Dictionary<VolteAddonInfo, string>();
        }

        public async Task InitAsync()
        {
            var sw = Stopwatch.StartNew();
            if (!Directory.Exists("addons")) return; //don't auto-create a directory; if someone wants to use addons they need to make it themselves.
            var addonFolders = Directory.GetDirectories("addons");
            if (addonFolders.IsEmpty())
            {
                _logger.Info(LogSource.Service, "No addons are in the addons directory; skipping initialization.");
                return;
            }
            
            foreach (var folder in addonFolders)
            {
                VolteAddonInfo meta = null;
                string code = null;
                foreach (var file in Directory.GetFiles(folder))
                {
                    if (file.EndsWith(".json"))
                    {
                        try
                        {
                            meta = JsonSerializer.Deserialize<VolteAddonInfo>(await File.ReadAllTextAsync(file),
                                Config.JsonOptions);
                            if (meta.Name.ToLower() is "list")
                            {
                                throw new InvalidOperationException(
                                    $"Addon with name {meta.Name} is being ignored because it is using a reserved name. Please change the name or remove it.");
                            }
                            
                        }
                        catch (JsonException e)
                        {
                            _logger.Error(LogSource.Service, $"Addon meta file '{file}' had invalid JSON contents.", e);
                        }
                        catch (InvalidOperationException e)
                        {
                            meta = null;
                            _logger.Error(LogSource.Service, e.Message);
                        }
                    }

                    if (file.EndsWith(".cs"))
                        code = await File.ReadAllTextAsync(file);
                }

                if (meta != null && code == null)
                    _logger.Error(LogSource.Service, $"Attempted to load addon {meta.Name} but there were no .cs files in its directory.");

                if (meta != null && code != null)
                    LoadedAddons.Add(meta, code);

            }
            
            var sopts = ScriptOptions.Default.WithImports(EvalHelper.Imports)
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic && !x.Location.IsNullOrWhitespace()));

            foreach (var (meta, code) in LoadedAddons)
            {
                try
                {
                    if ((await CSharpScript.RunAsync(code, sopts, new AddonEnvironment(_provider))).ReturnValue != null)
                        _logger.Info(LogSource.Service, "Addon's logic resulted in a value; ignoring.");
                }
                catch (Exception e)
                {
                    _logger.Error(LogSource.Service, $"Addon {meta.Name}'s logic produced an error.", e);
                }

            }
            sw.Stop();
            _logger.Info(LogSource.Service, $"{"addon".ToQuantity(LoadedAddons.Count)} loaded in {sw.Elapsed.Humanize()}.");
            
        }
    }
}