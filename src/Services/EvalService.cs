using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Qmmands;
using Qommon.Collections;
using Volte.Commands;
using Volte.Core.Models;

namespace Volte.Services
{
    public sealed class EvalService : VolteService
    {
        private readonly DatabaseService _db;
        private readonly LoggingService _logger;
        private readonly CommandService _commands;
        private readonly EmojiService _emoji;

        public EvalService(DatabaseService databaseService,
            LoggingService loggingService,
            CommandService commandService,
            EmojiService emojiService)
        {
            _db = databaseService;
            _logger = loggingService;
            _commands = commandService;
            _emoji = emojiService;
        }

        public async Task EvaluateAsync(VolteContext ctx, string code)
        {
            try
            {
                if (code.StartsWithIgnoreCase("```cs") && code.EndsWith("```"))
                {
                    code = code.Substring(5);
                    code = code.Remove(code.LastIndexOf("```", StringComparison.OrdinalIgnoreCase), 3);
                }

                await ExecuteScriptAsync(code, ctx);

            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Module, string.Empty, e);
            }
            finally
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                GC.WaitForPendingFinalizers();
            }
        }

        private EvalEnvironment CreateEvalEnvironment(VolteContext ctx)
            => new EvalEnvironment
            {
                Context = ctx,
                Client = ctx.Client.GetShardFor(ctx.Guild),
                Data = _db.GetData(ctx.Guild),
                Logger = _logger,
                CommandService = _commands,
                DatabaseService = _db,
                EmojiService = _emoji
            };

        private async Task ExecuteScriptAsync(string code, VolteContext ctx)
        {
            var sopts = ScriptOptions.Default.WithImports(_imports).WithReferences(
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => !x.IsDynamic && !x.Location.IsNullOrWhitespace()));

            var embed = ctx.CreateEmbedBuilder();
            var msg = await embed.WithTitle("Evaluating").WithDescription(Format.Code(code, "cs")).SendToAsync(ctx.Channel);
            try
            {
                var sw = Stopwatch.StartNew();
                var result = await CSharpScript.EvaluateAsync(code, sopts, CreateEvalEnvironment(ctx));
                sw.Stop();
                if (result is null)
                {
                    await msg.DeleteAsync();
                    await ctx.ReactSuccessAsync();
                }
                else
                {
                    var res = result switch
                        {
                        string str => str,
                        IEnumerable enumerable => enumerable.Cast<object>().Select(x => $"{x}").Join(", "),
                        _ => result.ToString()
                        };
                    await msg.ModifyAsync(m =>
                        m.Embed = embed.WithTitle("Eval")
                            .AddField("Elapsed Time", $"{sw.Elapsed.Humanize()}", true)
                            .AddField("Return Type", result.GetType(), true)
                            .WithDescription(Format.Code(res, "ini")).Build());
                }
            }
            catch (Exception e)
            {
                await msg.ModifyAsync(m =>
                    m.Embed = embed
                        .AddField("Exception Type", e.GetType(), true)
                        .AddField("Message", e.Message, true)
                        .WithTitle("Error")
                        .Build()
                );
            }
        }

        private readonly ReadOnlyList<string> _imports = new ReadOnlyList<string>(new ReadOnlyList<string>(new List<string>
        {
            "System", "System.Collections.Generic", "System.Linq", "System.Text",
            "System.Diagnostics", "Discord", "Discord.WebSocket", "System.IO", "Volte.Core.Models.EventArgs",
            "System.Threading", "Gommon", "Volte.Core.Models", "Humanizer", "System.Globalization",
            "Volte.Core", "Volte.Services", "System.Threading.Tasks", "Qmmands"
        }));
    }
}