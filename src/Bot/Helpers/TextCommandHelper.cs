using System.Collections;
using Volte.Systems.Commands.Text.Modules;
using Volte.Systems.Database.EntitiesV2;
using Module = Qmmands.Module;

namespace Volte.Helpers;

public static class TextCommandHelper
{
    public static async ValueTask<bool> CanShowCommandAsync(VolteContext ctx, Command command) =>
        await command.RunChecksAsync(ctx) is SuccessfulResult;

    public static async ValueTask<bool> CanShowModuleAsync(VolteContext ctx, Module module) =>
        await module.RunChecksAsync(ctx) is SuccessfulResult;

    public static string FormatCommandShort(Command command, bool includeGroup = true)
    {
        var firstAlias = command.FullAliases.FirstOrDefault();
        if (firstAlias is null) return null;

        if (!firstAlias.Contains(command.Service.Separator)) return Format.Code(firstAlias);

        return includeGroup
            ? Format.Bold(Format.Code(firstAlias))
            : Format.Bold(Format.Code(firstAlias.Split(command.Service.Separator)[1]));
    }

    public static string FormatModuleShort(Module module) => 
        module.FullAliases.FindFirst()
            .Convert(firstAlias => Format.Code(firstAlias))
            .OrElse(null);
        

    public static async IAsyncEnumerable<Command> WhereAccessibleAsync(this IEnumerable<Command> commands,
        VolteContext ctx)
    {
        foreach (var cmd in commands)
            if (await CanShowCommandAsync(ctx, cmd))
                yield return cmd;
    }

    public static bool IsAccessibleToGuild(this Command command, ulong id)
    {
        if (!command.Checks.Any(x => x is RequireSpecificGuildAttribute))
            return true;

        return command.Checks
                   .TryGetFirst(x => x is RequireSpecificGuildAttribute, out var attr)
               && attr is RequireSpecificGuildAttribute rsga
               && rsga.GuildIds.Contains(id);
    }

    public static async ValueTask<EmbedBuilder> CreateCommandEmbedAsync(Command command, VolteContext ctx)
    {
        var embed = ctx.CreateEmbedBuilder()
            .WithTitle(command.Name)
            .WithDescription(command.Description ?? "No description provided.");
        var checks = CommandUtilities.EnumerateAllChecks(command).ToList();

        if (command.Attributes.Any(x => x is DummyCommandAttribute))
        {
            await addSubcommandsFieldAsync();
            return checks.Count > 0
                ? embed.AddField("Checks",
                    (await Task.WhenAll(checks.Select(check => FormatCheckAsync(check, ctx)))).JoinToString("\n"))
                : embed;
        }

        if (command.Remarks != null)
            embed.AppendDescription($" {command.Remarks}");

        if (command.FullAliases.Any())
            embed.AddField("Aliases", command.FullAliases.Select(static x => Format.Code(x)).JoinToString(", "), true);

        if (command.Parameters.Any())
            embed.AddField("Parameters", command.Parameters.Select(FormatParameter).JoinToString("\n"));

        if (command.CustomArgumentParserType is null)
            embed.AddField("Usage", FormatUsage(ctx, command));

        if (command.Attributes.Any(x => x is ShowPlaceholdersInHelpAttribute))
            embed.AddField("Placeholders",
                WelcomeSettings.ValidPlaceholders
                    .Select(static x => $"{Format.Code($"{{{x.Key}}}")}: {Format.Italics(x.Value)}")
                    .JoinToString("\n"));

        if (command.Attributes.Any(x => x is ShowTimeFormatInHelpAttribute))
            embed.AddField("Example Valid Time",
                $"{Format.Code("4d3h2m1s")}: {Format.Italics("4 days, 3 hours, 2 minutes and one second.")}");

        if (command.Attributes.Any(x => x is ShowSubcommandsInHelpOverrideAttribute))
            await addSubcommandsFieldAsync();

        if (command.Attributes.TryGetFirst(x => x is ShowUnixArgumentsInHelpAttribute, out var unixAttr) 
            && unixAttr is ShowUnixArgumentsInHelpAttribute attr)
            embed.AddField("Unix Arguments", getArgs(attr.VolteUnixCommand));
            

        return checks.Count > 0
            ? embed.AddField("Checks",
                (await Task.WhenAll(
                    checks.Where(check => check is not RequireSpecificGuildAttribute)
                        .Select(check => FormatCheckAsync(check, ctx))
                    )
                ).JoinToString("\n"))
            : embed;
            
            
            
        async Task addSubcommandsFieldAsync()
        {
            embed.AddField("Subcommands", (await command.Module.Commands.WhereAccessibleAsync(ctx)
                    .Where(static x => !x.Attributes.Any(a => a is DummyCommandAttribute)).ToListAsync())
                .Select(static x => FormatCommandShort(x, false))
                .JoinToString(", "));
        }
            
        static string formatUnixArgs(KeyValuePair<string[], string> kvp) =>
            $"{Format.Bold(kvp.Key.Select(static name => $"-{name}").JoinToString(" or "))}: {kvp.Value}";

        static string getArgs(VolteUnixCommand unixCommand) => unixCommand switch
        {
            VolteUnixCommand.Announce => AdminUtilityModule.AnnounceNamedArguments.Select(formatUnixArgs)
                .JoinToString("\n"),
            VolteUnixCommand.Zalgo => UtilityModule.ZalgoNamedArguments.Select(formatUnixArgs).JoinToString("\n"),
            VolteUnixCommand.UnixBan => ModerationModule.UnixBanNamedArguments.Select(formatUnixArgs)
                .JoinToString("\n"),
            _ => throw new ArgumentOutOfRangeException(nameof(unixCommand))
        };
    }

    public static string FormatUsage(VolteContext ctx, Command cmd)
        => FormatUsage(ctx.GuildData.Settings.CommandPrefix, cmd);
    
    public static string FormatUsage(string commandPrefix, Command cmd)
    {
        return new StringBuilder($"{commandPrefix}{cmd.FullAliases[0].ToLower()} ")
            .Append(cmd.Parameters.Select(formatUsageParameter).JoinToString(" "))
            .ToString().Trim();

        static string formatUsageParameter(Parameter param)
            => String(sb =>
                sb.Append(param.IsOptional ? "[" : "{")
                    .Append(param.Name)
                    .Append(param.IsOptional ? "]" : "}")
            );
    }

    private static async Task<string> FormatCheckAsync(CheckAttribute cba, VolteContext context)
    {
        var result = await cba.CheckAsync(context);
        var message = GetCheckFriendlyMessage(context, cba);
        return $"- {(result.IsSuccessful ? Emojis.BallotBoxWithCheck : Emojis.X)} {message}";
    }

    private static string GetCheckFriendlyMessage(VolteContext ctx, CheckAttribute cba)
        => cba switch
        {
            RequireGuildAdminAttribute => "You need to have the Admin role.",
            RequireGuildModeratorAttribute => "You need to have the Moderator role.",
            RequireBotOwnerAttribute => $"Only usable by **{ctx.Client.GetOwner()}** (bot owner).",
            _ => $"Unimplemented check: {cba.GetType().AsPrettyString()}. Please report this to my developers :)"
        };

    private static string FormatParameter(Parameter param)
        => String(sb =>
        {
            sb.Append(Format.Code(param.Name));
                
            if (!param.Description.IsNullOrWhitespace())
                sb.Append($": {param.Description} ");
            if (param.Checks.Any(static x => x is EnsureNotSelfAttribute))
                sb.Append("Cannot be yourself.");
            if (param.DefaultValue != null)
                sb.Append($"Defaults to: {Format.Code(param.DefaultValue.ToString())}");
        }).Trim();

    internal static IEnumerable<Type> AddTypeParsers(this CommandService service)
    {
        var parsers = Assembly.GetExecutingAssembly().ExportedTypes
            .Where(static x => x.HasAttribute<InjectTypeParserAttribute>())
            .ToList();

        var csMirror = Mirror.Reflect(service);

        foreach (var parser in parsers)
        {
            csMirror.CallGeneric("AddTypeParser", 
                parser.BaseType!.GenericTypeArguments,
                BindingFlags.Public,
                args: [
                    parser.GetConstructor(Type.EmptyTypes)?.Invoke([]) ?? throw new InvalidOperationException($"Couldn't find no-args constructor for {parser.AsFullNamePrettyString()}"), 
                    parser.GetCustomAttribute<InjectTypeParserAttribute>()!.OverridePrimitive
                ]
            );
                
            yield return parser;
        }
    }

    public static Command GetCommand(this CommandService service, string name)
        => service.FindCommands(name).FirstOrDefault()?.Command;

    public static int GetTotalTypeParsers(this CommandService cs)
    {
        var customParsers = Assembly.GetExecutingAssembly().GetTypes()
            .Count(x => x.HasAttribute<InjectTypeParserAttribute>());

        var primitiveParsers = Mirror.ReflectUnsafe(cs).Get<IDictionary>("_primitiveTypeParsers").Count;
            
        return customParsers + primitiveParsers;
    }
}