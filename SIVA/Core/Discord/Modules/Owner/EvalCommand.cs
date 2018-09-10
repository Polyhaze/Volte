using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Owner {
    public class EvalCommand : SIVACommand {

        private ScriptOptions _scriptOptions;
        
        [Command("Eval")]
        public async Task Eval(string code) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.Message, RawEmoji.X);
                return;
            }

            var result = "No result.";

            CreateScriptOptions();

            try {
                var evalRes = await CSharpScript.EvaluateAsync<object>(code, _scriptOptions);
                result = evalRes.ToString();
            }
            catch (Exception e) {
                result = e.ToString();
            }

            await Context.Channel.SendMessageAsync("", false, CreateEmbed(Context, $"{result}"));

        }

        private void CreateScriptOptions() {
            var dd = typeof(object).GetTypeInfo().Assembly.Location;
            var coreDir = Directory.GetParent(dd);

            var references = new List<MetadataReference> {
                MetadataReference.CreateFromFile($"{coreDir.FullName}{Path.DirectorySeparatorChar}mscorlib.dll"),
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location)
            };

            var referencedAssemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
            foreach (var referenced in referencedAssemblies) {
                var loadedAssembly = Assembly.Load(referenced);
                references.Add(MetadataReference.CreateFromFile(loadedAssembly.Location));
            }

            _scriptOptions = ScriptOptions.Default
                .AddImports("System", "System.Linq", "System.Text", "Discord", "Discord.WebSocket")
                .AddReferences(references);
        }
    }
}