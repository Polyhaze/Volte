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
using Volte.Commands;
using Volte.Core.Models;

namespace Volte.Services
{
    [Service("Eval", "Handles C# code evaluation.")]
    public sealed class EvalService
    {
        private readonly DatabaseService _db;
        private readonly LoggingService _logger;
        private readonly BinService _bin;
        private readonly CommandService _commands;
        private readonly EmojiService _emoji;

        public EvalService(DatabaseService databaseService,
            LoggingService loggingService,
            BinService binService,
            CommandService commandService,
            EmojiService emojiService)
        {
            _db = databaseService;
            _logger = loggingService;
            _bin = binService;
            _commands = commandService;
            _emoji = emojiService;
        }

        public async Task EvaluateAsync(VolteContext ctx, string code)
        {
            try
            {
                var sopts = ScriptOptions.Default;
                var embed = ctx.CreateEmbedBuilder();
                if (code.Contains("```cs"))
                {
                    code = code.Remove(code.IndexOf("```cs", StringComparison.OrdinalIgnoreCase), 5);
                    code = code.Remove(code.LastIndexOf("```", StringComparison.OrdinalIgnoreCase), 3);
                }

                var objects = new EvalObjects
                {
                    Context = ctx,
                    Client = ctx.Client,
                    Data = _db.GetData(ctx.Guild),
                    Logger = _logger,
                    CommandService = _commands,
                    BinService = _bin,
                    DatabaseService = _db,
                    EmojiService = _emoji
                };

                sopts = sopts.WithImports(_imports).WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => !x.IsDynamic && !x.Location.IsNullOrWhitespace()));

                var msg = await embed.WithTitle("Evaluating...").SendToAsync(ctx.Channel);
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
                        await ctx.ReactSuccessAsync();
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
                await _logger.LogAsync(LogSeverity.Error, LogSource.Module, string.Empty, e);
            }
        }

        private readonly List<string> _imports = new List<string>
        {
            "System", "System.Collections.Generic", "System.Linq", "System.Text",
            "System.Diagnostics", "Discord", "Discord.WebSocket", "System.IO",
            "System.Threading", "Gommon", "Volte.Core.Models", "Humanizer", "System.Globalization",
            "Volte.Core", "Volte.Services", "System.Threading.Tasks", "Qmmands"
        };
    }
}