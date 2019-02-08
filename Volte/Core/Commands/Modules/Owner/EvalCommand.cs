using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Data;
using Volte.Core.Data.Objects;
using Volte.Core.Discord;
using Volte.Core.Extensions;
using Volte.Core.Services;

namespace Volte.Core.Commands.Modules.Owner {
    public partial class OwnerModule : VolteModule {

        [Command("Eval")]
        [Summary("Evaluates C# code.")]
        [Remarks("Usage: |prefix|eval {code}")]
        [RequireBotOwner]
        public async Task Eval([Remainder] string code) {
            try {
                object res;
                var sopts = ScriptOptions.Default;
                var embed = new EmbedBuilder()
                    .WithAuthor(Context.User)
                    .WithColor(Config.GetSuccessColor());
                if (code.Contains("```cs")) {
                    code = code.Remove(code.IndexOf("```cs", StringComparison.Ordinal), 5);
                    code = code.Remove(code.LastIndexOf("```", StringComparison.Ordinal), 3);
                }

                var objects = new EvalObjects {
                    Context = Context,
                    CommandService = VolteBot.CommandService,
                    Config = Db.GetConfig(Context.Guild),
                    DatabaseService = Db,
                    DebugService = VolteBot.ServiceProvider.GetRequiredService<DebugService>(),
                    Logger = VolteBot.ServiceProvider.GetRequiredService<LoggingService>()
                };

                var imports = new[] {
                    "System", "System.Collections.Generic", "System.Linq", "System.Text", "System.Threading.Tasks",
                    "System.Diagnostics", "Discord", "Discord.Commands", "Discord.WebSocket"
                };

                sopts = sopts.WithImports(imports).WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)));

                var msg = await embed.WithTitle("Evaluating...").SendTo(Context.Channel);
                var sw = new Stopwatch();
                sw.Start();
                try {
                    res = await CSharpScript.EvaluateAsync(code, sopts, objects, typeof(EvalObjects));
                    sw.Stop();
                    if (res != null) {
                        await msg.DeleteAsync();
                        await embed.WithTitle("Eval")
                            .AddField("Elapsed Time", $"{sw.ElapsedMilliseconds}ms")
                            .AddField("Input", Format.Code(code, "cs"))
                            .AddField("Output", Format.Code(code, "cs"))
                            .SendTo(Context.Channel);
                    }
                    else {
                        await msg.DeleteAsync();
                        await embed.WithTitle("Eval")
                            .AddField("Elapsed Time", $"{sw.ElapsedMilliseconds}ms")
                            .AddField("Input", Format.Code(code, "cs"))
                            .AddField("Output", "No output.")
                            .SendTo(Context.Channel);
                    }
                }
                catch (Exception e) {
                    await msg.ModifyAsync(m =>
                        m.Embed = embed
                            .WithDescription($"`{e.Message}`")
                            .WithTitle("Error")
                            .Build());
                    File.WriteAllText("data/EvalError.log", $"{e.Message}\n{e.StackTrace}");
                    await Context.Channel.SendFileAsync("data/EvalError.log");
                    File.Delete("data/EvalError.log");
                }
                finally {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

            }
            catch (Exception e) {
                Logger.Log(LogSeverity.Error, "Module", string.Empty, e);
            }
        }
        
    }
}