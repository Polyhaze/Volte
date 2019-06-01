using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models;
using Volte.Extensions;

namespace Volte.Commands.Modules.BotOwner
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("Eval", "Evaluate")]
        [Description("Evaluates C# code.")]
        [Remarks("Usage: |prefix|eval {code}")]
        [RequireBotOwner]
        public Task EvalAsync([Remainder] string code)
        {
            _ = Executor.ExecuteAsync(async () =>
            {
                try
                {
                    var sopts = ScriptOptions.Default;
                    var embed = Context.CreateEmbedBuilder();
                    if (code.Contains("```cs"))
                    {
                        code = code.Remove(code.IndexOf("```cs", StringComparison.OrdinalIgnoreCase), 5);
                        code = code.Remove(code.LastIndexOf("```", StringComparison.OrdinalIgnoreCase), 3);
                    }

                    var objects = new EvalObjects
                    {
                        Context = Context,
                        Client = Context.Client,
                        Data = Db.GetData(Context.Guild),
                        Logger = Logger,
                        CommandService = CommandService,
                        BinService = BinService,
                        DatabaseService = Db,
                        EmojiService = EmojiService
                    };

                    sopts = sopts.WithImports(_imports).WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                        .Where(x => !x.IsDynamic && !x.Location.IsNullOrWhitespace()));

                    var msg = await embed.WithTitle("Evaluating...").SendToAsync(Context.Channel);
                    try
                    {
                        var sw = Stopwatch.StartNew();
                        var res = await CSharpScript.EvaluateAsync(code, sopts, objects);
                        sw.Stop();
                        if (res != null)
                        {
                            await msg.ModifyAsync(m =>
                                m.Embed = embed.WithTitle("Eval")
                                    .AddField("Elapsed Time", $"{sw.ElapsedMilliseconds}ms", true)
                                    .AddField("Return Type", res.GetType().FullName, true)
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
                                .AddField("Exception Type", e.GetType().FullName, true)
                                .AddField("Message", e.Message, true)
                                .WithDescription($"{Format.Code(e.StackTrace, "cs")}")
                                .WithTitle("Error")
                                .Build()
                        );
                    }
                    finally
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                catch (Exception e)
                {
                    await Logger.LogAsync(LogSeverity.Error, LogSource.Module, string.Empty, e);
                }
            });

            return Task.CompletedTask;
        }

        private readonly List<string> _imports = new List<string>
        {
            "System", "System.Collections.Generic", "System.Linq", "System.Text",
            "System.Diagnostics", "Discord", "Discord.WebSocket", "System.IO",
            "System.Threading", "Volte.Extensions", "Gommon", "Volte.Data", "Humanizer",
            "Volte.Core", "Volte.Services", "System.Threading.Tasks", "Qmmands"
        };
    }
}