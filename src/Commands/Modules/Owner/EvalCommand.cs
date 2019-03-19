using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Volte.Commands.Preconditions;
using Volte.Data.Objects;
using Volte.Extensions;
using Volte.Utils;
using Discord;
using Qmmands;

namespace Volte.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        [Command("Eval")]
        [Description("Evaluates C# code.")]
        [Remarks("Usage: |prefix|eval {code}")]
        [RequireBotOwner]
        public async Task EvalAsync([Remainder] string code)
        {
            await ExecutorUtil.ExecuteAsync(async () =>
            {
                try
                {
                    var sopts = ScriptOptions.Default;
                    var embed = Context.CreateEmbedBuilder();
                    if (code.Contains("```cs"))
                    {
                        code = code.Remove(code.IndexOf("```cs", StringComparison.Ordinal), 5);
                        code = code.Remove(code.LastIndexOf("```", StringComparison.Ordinal), 3);
                    }

                    var objects = new EvalObjects
                    {
                        Context = Context,
                        Client = Context.Client,
                        Config = Db.GetConfig(Context.Guild),
                        Logger = Logger,
                        CommandService = CommandService,
                        DebugService = DebugService,
                        DatabaseService = Db,
                        EmojiService = EmojiService
                    };

                    var imports = new[]
                    {
                        "System", "System.Collections.Generic", "System.Linq", "System.Text",
                        "System.Diagnostics", "Discord", "Discord.WebSocket", "System.IO",
                        "System.Threading", "Volte.Extensions", "Volte.Utils", "Volte.Data",
                        "Volte.Discord", "Volte.Services", "System.Threading.Tasks", "Qmmands"
                    };

                    sopts = sopts.WithImports(imports).WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                        .Where(x => !x.IsDynamic && !x.Location.IsNullOrWhitespace()));

                    var msg = await embed.WithTitle("Evaluating...").SendToAsync(Context.Channel);
                    try
                    {
                        var sw = Stopwatch.StartNew();
                        var res = await CSharpScript.EvaluateAsync(code, sopts, objects);
                        sw.Stop();
                        if (res != null)
                        {
                            await msg.ModifyAsync(m => m.Embed = embed.WithTitle("Eval")
                                .AddField("Elapsed Time", $"{sw.ElapsedMilliseconds}ms")
                                .AddField("Input", Format.Code(code, "cs"))
                                .AddField("Output", Format.Code(res.ToString(), "css")).Build());
                        }
                        else
                        {
                            await msg.DeleteAsync();
                            await Context.ReactSuccessAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        await msg.ModifyAsync(m =>
                            m.Embed = embed
                                .WithDescription($"`{e.Message}`")
                                .WithTitle("Error")
                                .Build()
                        );
                        File.WriteAllText("data/EvalError.log", $"{e.Message}\n{e.StackTrace}");
                        await Context.Channel.SendFileAsync("data/EvalError.log", string.Empty);
                        File.Delete("data/EvalError.log");
                    }
                    finally
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                catch (Exception e)
                {
                    await Logger.Log(LogSeverity.Error, LogSource.Module, string.Empty, e);
                }
            });
        }
    }
}