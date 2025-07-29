using System.Collections;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Volte.Systems.Database;

namespace Volte.Systems.Eval;

public static partial class EvalHelper
{
    public static readonly string[] BaseImports =
    [
        "System", "System.IO", "System.Linq", "System.Threading", "System.Threading.Tasks",
        "System.Collections.Generic", "System.Diagnostics", "System.Globalization", "System.Net.Http",
        "System.Text", "System.Text.Json", "System.Text.Json.Serialization",

        "Discord", "Discord.WebSocket",

        "Humanizer", "Gommon", "Qmmands"
    ];

    private static string[] _resolvedImports;

    public static readonly ScriptOptions Options = ScriptOptions.Default
        .WithImports(GetImports())
        .WithReferences(
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(static x => !x.IsDynamic && !x.Location.IsNullOrWhitespace())
        );

    public static string[] GetImports() =>
        _resolvedImports ??= BaseImports
            .Concat(
                typeof(EvalHelper).Assembly
                    .GetTypes()
                    .Select(static x => x.Namespace)
                    .Where(static x => x != null)
                    .Distinct()
                    .Except(BaseImports)
            )
            .Distinct()
            .ToArray();

    public static Task EvaluateAsync(VolteContext ctx, string code)
    {
        try
        {
            if (Pattern.IsMatch(code, out var match))
                code = match.Groups[1].Value;

            return ExecuteScriptAsync(code, ctx);
        }
        catch (Exception e)
        {
            Error(LogSource.Module, string.Empty, e);
        }
        finally
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
            GC.WaitForPendingFinalizers();
        }

        return Task.CompletedTask;
    }

    private static EvalEnvironment CreateEvalEnvironment(VolteContext ctx) =>
        new()
        {
            Context = ctx,
            Database = ctx.Services.Get<DatabaseService>(),
            Client = ctx.Client,
            Data = ctx.Services.Get<DatabaseService>().GetData(ctx.Guild),
            Commands = ctx.Services.Get<CommandService>()
        };

    private static async Task ExecuteScriptAsync(string code, VolteContext ctx)
    {
        var embed = ctx.CreateEmbedBuilder();
        var msg = await embed.WithTitle("Evaluating")
            .WithDescription(Format.Code(code, "cs"))
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
                        await eb.SendToAsync(env.Context.Channel);
                        break;
                    case Embed e:
                        shouldReply = false;
                        await e.SendToAsync(env.Context.Channel);
                        break;
                }

                if (!shouldReply)
                {
                    await msg.DeleteAsync().Then(() => env.Context.Message.AddReactionAsync(Emojis.BallotBoxWithCheck));
                    return;
                }


                var res = state.ReturnValue switch
                {
                    bool b => b.ToString().ToLower(),
                    string str => str,
                    IEnumerable enumerable and not string => enumerable.Cast<object>().ToReadableString(),
                    IUser user => $"{user} ({user.Id})",
                    ITextChannel channel => $"#{channel.Name} ({channel.Id})",
                    IMessage message => env.Inspect(message),
                    _ => state.ReturnValue.ToString()
                };

                await msg.ModifyAsync(m =>
                    m.Embed = embed.WithTitle("Eval")
                        .AddField("Elapsed Time", $"{sw.Elapsed.Humanize()}", true)
                        .AddField("Return Type", state.ReturnValue.GetType().AsPrettyString(), true)
                        .WithDescription(Format.Code(res, res.IsNullOrEmpty() ? string.Empty : "ini")).Build());
            }
            else
                await msg.DeleteAsync().Then(() => env.Context.Message.AddReactionAsync(Emojis.BallotBoxWithCheck));
        }
        catch (Exception ex)
        {
            ex.SentryCapture(scope =>
                scope.AddBreadcrumb("This exception comes from a dynamically executed C# script.")
            );

            await msg.ModifyAsync(m =>
                m.Embed = embed
                    .AddField("Exception Type", ex.GetType().AsPrettyString(), true)
                    .AddField("Message", ex.Message, true)
                    .WithTitle("Error")
                    .Build()
            );
        }
    }

    private static readonly Regex Pattern = CodeInputPattern();

    [GeneratedRegex("[\t\n\r]*`{3}(?:cs)?[\n\r]+((?:.|\n|\t\r)+)`{3}",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex CodeInputPattern();
}