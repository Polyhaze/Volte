using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Qmmands;
using Qommon.Collections;
using Sentry;
using Volte.Commands;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Core.Helpers
{
    public static class EvalHelper
    {
        private static readonly Regex Pattern = new Regex("[\t\n\r]*`{3}(?:cs)?[\n\r]+((?:.|\n|\t\r)+)`{3}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static readonly ReadOnlyList<string> Imports = new ReadOnlyList<string>(new List<string>
        {
            "System", "System.IO", "System.Linq", "System.Text", "System.Threading", "System.Threading.Tasks",
            "System.Collections.Generic", "System.Diagnostics", "System.Globalization",

            "Volte.Core", "Volte.Core.Helpers", "Volte.Core.Entities", "Volte.Commands",

            "Discord", "Discord.WebSocket",

            "Humanizer", "Gommon", "Qmmands"
        });
        
        public static readonly ScriptOptions Options = ScriptOptions.Default.WithImports(Imports)
            .WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && !x.Location.IsNullOrWhitespace()));

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
                Logger.Error(LogSource.Module, string.Empty, e);
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
                Commands = ctx.Services.Get<CommandService>()
            };

        private static async Task ExecuteScriptAsync(string code, VolteContext ctx)
        {
            var embed = ctx.CreateEmbedBuilder();
            var msg = await embed.WithTitle("Evaluating").WithDescription(Format.Code(code, "cs"))
                .SendToAsync(ctx.Channel);
            try
            {
                var env = CreateEvalEnvironment(ctx);
                var sw = Stopwatch.StartNew();
                var state = await CSharpScript.RunAsync(code, Options, env);
                sw.Stop();
                var shouldReply = true;
                if (state.ReturnValue != null)
                {
                    switch (state.ReturnValue)
                    {
                        case EmbedBuilder eb:
                            shouldReply = false;
                            await env.ReplyAsync(eb);
                            break;
                        case Embed e:
                            shouldReply = false;
                            await env.ReplyAsync(e);
                            break;
                    }
                    
                    var res = state.ReturnValue switch
                    {
                        bool b => b.ToString().ToLower(),
                        IEnumerable enumerable when !(state.ReturnValue is string) => enumerable.Cast<object>().ToReadableString(),
                        IUser user => $"{user} ({user.Id})",
                        ITextChannel channel => $"#{channel.Name} ({channel.Id})",
                        IMessage message => env.Inspect(message),
                        _ => state.ReturnValue.ToString()
                    };
                    if (shouldReply) await msg.ModifyAsync(m =>
                        m.Embed = embed.WithTitle("Eval")
                            .AddField("Elapsed Time", $"{sw.Elapsed.Humanize()}", true)
                            .AddField("Return Type", state.ReturnValue.GetType().AsPrettyString(), true)
                            .WithDescription(Format.Code(res, res.IsNullOrEmpty() ? string.Empty : "ini")).Build());
                }
                else
                    await msg.DeleteAsync().ContinueWith(_ => env.ReactAsync(DiscordHelper.BallotBoxWithCheck));
            }
            catch (Exception ex)
            {
                SentrySdk.AddBreadcrumb("This exception comes from an eval.");
                SentrySdk.CaptureException(ex);
                await msg.ModifyAsync(m =>
                    m.Embed = embed
                        .AddField("Exception Type", ex.GetType().AsPrettyString(), true)
                        .AddField("Message", ex.Message, true)
                        .WithTitle("Error")
                        .Build()
                );
            }
        }
    }
}