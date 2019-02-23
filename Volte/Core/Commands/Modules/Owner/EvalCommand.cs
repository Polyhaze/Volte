using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Data;
using Volte.Core.Data.Objects;
using Volte.Core.Extensions;
using Volte.Core.Services;

namespace Volte.Core.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        public new LoggingService Logger { get; set; }

        [Command("Eval")]
        [Description("Evaluates C# code.")]
        [Remarks("Usage: |prefix|eval {code}")]
        [RequireBotOwner]
        public Task EvalAsync([Remainder] string code)
        {
            new Thread(async () =>
            {
                try
                {
                    var sopts = ScriptOptions.Default;
                    var embed = new EmbedBuilder()
                        .WithAuthor(Context.User)
                        .WithColor(Config.GetSuccessColor());
                    if (code.Contains("```cs"))
                    {
                        code = code.Remove(code.IndexOf("```cs", StringComparison.Ordinal), 5);
                        code = code.Remove(code.LastIndexOf("```", StringComparison.Ordinal), 3);
                    }

                    var objects = new EvalObjects
                    {
                        Context = Context,
                        Client = Context.Client,
                        CommandService = CommandService,
                        Config = Db.GetConfig(Context.Guild),
                        DatabaseService = Db,
                        DebugService = DebugService,
                        Logger = Logger
                    };

                    var imports = new[]
                    {
                        "System", "System.Collections.Generic", "System.Linq", "System.Text", "System.Threading.Tasks",
                        "System.Diagnostics", "Discord", "Qmmands", "Discord.WebSocket", "System.IO", "System.Threading"
                    };

                    sopts = sopts.WithImports(imports).WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                        .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)));

                    var msg = await embed.WithTitle("Evaluating...").SendTo(Context.Channel);
                    try
                    {
                        var sw = Stopwatch.StartNew();
                        var res = await CSharpScript.EvaluateAsync(code, sopts, objects, typeof(EvalObjects));
                        sw.Stop();
                        if (res != null && res.GetType() != typeof(AsyncTaskMethodBuilder))
                        {
                            await msg.ModifyAsync(m => m.Embed = embed.WithTitle("Eval")
                                .AddField("Elapsed Time", $"{sw.ElapsedMilliseconds}ms")
                                .AddField("Input", Format.Code(code, "cs"))
                                .AddField("Output", Format.Code(res.ToString(), "cs")).Build());
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
            }).Start();
            return Task.CompletedTask;
        }
    }
}