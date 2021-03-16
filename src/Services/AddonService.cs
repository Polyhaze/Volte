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

namespace Volte.Services
{
    public class AddonService : VolteService
    {
        private readonly EvalService _eval;
        private readonly LoggingService _logger;
        private readonly IServiceProvider _provider;
        public Dictionary<VolteAddonInfo, string> Addons { get; }

        public AddonService(EvalService evalService,
            LoggingService loggingService,
            IServiceProvider serviceProvider)
        {
            _eval = evalService;
            _logger = loggingService;
            _provider = serviceProvider;
            Addons = new Dictionary<VolteAddonInfo, string>();
        }

        public async Task InitAsync()
        {
            var sw = Stopwatch.StartNew();
            foreach (var folder in Directory.GetDirectories("addons"))
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
                        }
                        catch (Exception e)
                        {
                            _logger.Error(LogSource.Service, $"Addon meta file '{file}' had invalid JSON contents.", e);
                        }
                    }

                    if (file.EndsWith(".cs"))
                    {
                        code = await File.ReadAllTextAsync(file);
                    }
                }

                if (meta != null && code == null)
                {
                    _logger.Error(LogSource.Service, $"Attempted to load addon {meta.Name} but there were no .cs files in its directory.");
                }

                if (meta != null && code != null)
                {
                    Addons.Add(meta, code);
                }

            }
            
            var sopts = ScriptOptions.Default.WithImports(_eval.Imports)
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic && !x.Location.IsNullOrWhitespace()));

            foreach (var (_, code) in Addons)
            {
                var state = await CSharpScript.RunAsync(code, sopts, new AddonEnvironment(_provider));
                if (state.ReturnValue != null)
                {
                    _logger.Info(LogSource.Service, "Addon's logic resulted in a value; ignoring.");
                }
            }
            
            _logger.Info(LogSource.Service, $"{"addon".ToQuantity(Addons.Count)} loaded in {sw.Elapsed.Humanize()}.");
            
        }
    }
}