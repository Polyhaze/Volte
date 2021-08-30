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
using Sentry;
using Volte.Commands;
using Volte.Interactions;
using Volte.Entities;
using Volte.Services;

namespace Volte.Helpers
{
    public static class EvalHelper
    {
        private static readonly Regex Pattern = new Regex("[\t\n\r]*`{3}(?:cs)?[\n\r]+((?:.|\n|\t\r)+)`{3}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static readonly ReadOnlyList<string> Imports = new ReadOnlyList<string>(new []
        {
            "System", "System.IO", "System.Linq", "System.Text", "System.Threading", "System.Threading.Tasks",
            "System.Collections.Generic", "System.Diagnostics", "System.Globalization", "System.Net.Http",

            "Volte", "Volte.Helpers", "Volte.Entities", "Volte.Commands", "Volte.Services", "Volte.Interactions",

            "Discord", "Discord.WebSocket",

            "Humanizer", "Gommon", "Qmmands"
        });
        
        public static readonly ScriptOptions Options = ScriptOptions.Default.WithImports(Imports)
            .WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && !x.Location.IsNullOrWhitespace()));

        public static Task EvaluateAsync(object ctx, string code)
        {
            try
            {
                if (Pattern.IsMatch(code, out var match))
                    code = match.Groups[1].Value;
                

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

        private static EvalEnvironment.InteractionBased CreateInteractionEvalEnvironment(MessageCommandContext ctx) =>
            new EvalEnvironment.InteractionBased
            {
                Context = ctx,
                Database = ctx.Services.Get<DatabaseService>(),
                Client = ctx.Client.GetShardFor(ctx.Guild),
                Data = ctx.Services.Get<DatabaseService>().GetData(ctx.Guild),
                Interactions = ctx.Services.Get<InteractionService>()
            };

        private static async Task ExecuteScriptAsync(string code, object ctx)
        {
            var embed = (ctx is VolteContext vctx)
                ? vctx.CreateEmbedBuilder()
                : ctx.Cast<MessageCommandContext>().CreateEmbedBuilder();

            var channel = (ctx is VolteContext)
                ? ctx.Cast<VolteContext>().Channel
                : ctx.Cast<MessageCommandContext>().TextChannel;
            
            try
            {
                var env = (ctx is VolteContext)
                    ? CreateEvalEnvironment(ctx.Cast<VolteContext>())
                    : CreateInteractionEvalEnvironment(ctx.Cast<MessageCommandContext>()).Cast<object>();

                
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
                            await (env is EvalEnvironment.InteractionBased ienv1
                                ? eb.SendToAsync(ienv1.Context.Channel)
                                : eb.SendToAsync(env.Cast<EvalEnvironment>().Context.Channel));
                            break;
                        case Embed e:
                            shouldReply = false;
                            await (env is EvalEnvironment.InteractionBased ienv2
                                ? e.SendToAsync(ienv2.Context.Channel)
                                : e.SendToAsync(env.Cast<EvalEnvironment>().Context.Channel));
                            break;
                    }

                    var res = state.ReturnValue switch
                    {
                        bool b => b.ToString().ToLower(),
                        IEnumerable enumerable when !(state.ReturnValue is string) => enumerable.Cast<object>()
                            .ToReadableString(),
                        IUser user => $"{user} ({user.Id})",
                        ITextChannel tc => $"#{tc.Name} ({tc.Id})",
                        _ => state.ReturnValue.ToString()
                    };

#pragma warning disable 8509 | only 2 possible types for this variable.
                    await (env switch
#pragma warning restore 8509
                    {
                        EvalEnvironment tenv => shouldReply
                            ? tenv.Context.CreateEmbedBuilder().WithTitle("Eval")
                                .AddField("Elapsed Time", $"{sw.Elapsed.Humanize()}", true)
                                .AddField("Return Type", state.ReturnValue.GetType().AsPrettyString(), true)
                                .WithDescription(Format.Code(res, res.IsNullOrEmpty() ? string.Empty : "ini"))
                                .SendToAsync(tenv.Context.Channel)
                            : tenv.ReactAsync(DiscordHelper.BallotBoxWithCheck),
                        
                        EvalEnvironment.InteractionBased ienv => shouldReply
                            ? ienv.Context.CreateReplyBuilder(true)
                                .WithEmbeds(embed.WithTitle("Eval")
                                    .AddField("Elapsed Time", $"{sw.Elapsed.Humanize()}", true)
                                    .AddField("Return Type", state.ReturnValue.GetType().AsPrettyString(), true)
                                    .WithDescription(Format.Code(res, res.IsNullOrEmpty() ? string.Empty : "ini")))
                                .RespondAsync()
                            : ienv.Context.CreateReplyBuilder(true)
                                .WithEmbedFrom("Eval succeeded.")
                                .RespondAsync()
                    });
                }
            }
            catch (Exception ex)
            {
                SentrySdk.AddBreadcrumb("This exception comes from an eval.");

                switch (ctx)
                {
                    case VolteContext vctx2:
                        await embed
                            .AddField("Exception Type", ex.GetType().AsPrettyString(), true)
                            .AddField("Message", ex.Message, true)
                            .WithTitle("Error")
                            .SendToAsync(vctx2.Channel);
                        break;
                    case MessageCommandContext mctx:
                        await mctx.CreateReplyBuilder(true)
                            .WithEmbeds(embed
                                .AddField("Exception Type", ex.GetType().AsPrettyString(), true)
                                .AddField("Message", ex.Message, true)
                                .WithTitle("Error"))
                            .RespondAsync();
                        break;
                }
                
                SentrySdk.CaptureException(ex);
            }
        }
    }
}