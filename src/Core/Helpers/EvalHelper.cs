using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Qmmands;
using Qommon.Collections;
using Volte.Commands;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Core.Helpers
{
    public static class EvalHelper
    {
        private static readonly Regex Pattern = new Regex("[\t\n\r]*`{3}(?:cs)?[\n\r]+((?:.|\n|\t\r)+)`{3}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static Task EvaluateAsync(VolteContext ctx, string code)
        {
            try
            {
                if (Pattern.IsMatch(code, out var match))
                {
                    code = match.Groups[1].Value;
                }

                return ExecuteScriptAsync(code, ctx);
            }
            catch (Exception e)
            {
                ctx.Services.Get<LoggingService>().Error(LogSource.Module, string.Empty, e);
            }
            finally
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                GC.WaitForPendingFinalizers();
            }

            return Task.CompletedTask;
        }

        private static EvalEnvironment CreateEvalEnvironment(VolteContext ctx) =>
            new EvalEnvironment
            {
                Context = ctx,
                Database = ctx.Services.Get<DatabaseService>(),
                Client = ctx.Client.GetShardFor(ctx.Guild),
                Data = ctx.Services.Get<DatabaseService>().GetData(ctx.Guild),
                Logger = ctx.Services.Get<LoggingService>(),
                Commands = ctx.Services.Get<CommandService>(),
            };

        private static async Task ExecuteScriptAsync(string code, VolteContext ctx)
        {
            var sopts = ScriptOptions.Default.WithImports(Imports)
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic && !x.Location.IsNullOrWhitespace()));

            var embed = ctx.CreateEmbedBuilder();
            var msg = await embed.WithTitle("Evaluating").WithDescription(Format.Code(code, "cs"))
                .SendToAsync(ctx.Channel);
            try
            {
                var sw = Stopwatch.StartNew();
                var state = await CSharpScript.RunAsync(code, sopts, CreateEvalEnvironment(ctx));
                sw.Stop();
                if (state.ReturnValue is null)
                {
                    await msg.DeleteAsync();
                    await ctx.Message.AddReactionAsync(DiscordHelper.BallotBoxWithCheck.ToEmoji());
                }
                else
                {
                    var res = state.ReturnValue switch
                    {
                        string str => str,
                        IEnumerable enumerable => enumerable.Cast<object>().ToReadableString(),
                        IUser user => $"{user} ({user.Id})",
                        ITextChannel channel => $"#{channel.Name} ({channel.Id})",
                        _ => state.ReturnValue.ToString()
                    };
                    await msg.ModifyAsync(m =>
                        m.Embed = embed.WithTitle("Eval")
                            .AddField("Elapsed Time", $"{sw.Elapsed.Humanize()}", true)
                            .AddField("Return Type", state.ReturnValue.GetType().AsPrettyString(), true)
                            .WithDescription(Format.Code(res, "ini")).Build());
                }
            }
            catch (Exception ex)
            {
                await msg.ModifyAsync(m =>
                    m.Embed = embed
                        .AddField("Exception Type", ex.GetType().AsPrettyString(), true)
                        .AddField("Message", ex.Message, true)
                        .WithTitle("Error")
                        .Build()
                );
            }
        }

        public static readonly ReadOnlyList<string> Imports = new ReadOnlyList<string>(new List<string>
            {
                "System", "System.Collections.Generic", "System.Linq", "System.Text", "Volte.Commands.TypeParsers",
                "System.Diagnostics", "Discord", "Discord.WebSocket", "System.IO", "Humanizer", 
                "System.Threading", "Gommon", "Volte.Core.Entities", "System.Globalization",
                "Volte.Core", "Volte.Services", "System.Threading.Tasks", "Qmmands", "Volte.Core.Helpers"
            });
    }
}