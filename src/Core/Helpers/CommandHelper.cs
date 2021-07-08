using Discord;
using Gommon;
using Humanizer;
using Qmmands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volte.Commands;
using Volte.Commands.Modules;
using Volte.Core.Entities;
using Module = Qmmands.Module;

namespace Volte.Core.Helpers
{
    public static class CommandHelper
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

        public static string FormatModuleShort(Module module)
        {
            var firstAlias = module.FullAliases.FirstOrDefault();
            if (firstAlias is null) return null;

            return Format.Bold(Format.Code(firstAlias));
        }

        public static async IAsyncEnumerable<Command> WhereAccessibleAsync(this IEnumerable<Command> commands,
            VolteContext ctx)
        {
            foreach (var cmd in commands)
            {
                if (await CanShowCommandAsync(ctx, cmd))
                    yield return cmd;
            }
        }

        public static async ValueTask<EmbedBuilder> CreateCommandEmbedAsync(Command command, VolteContext ctx)
        {
            var embed = ctx.CreateEmbedBuilder()
                .WithTitle(command.Name)
                .WithDescription(command.Description ?? "No description provided.");

            if (command.Attributes.Any(x => x is DummyCommandAttribute))
            {
                embed.AddField("Subcommands", (await command.Module.Commands.WhereAccessibleAsync(ctx)
                        .Where(x => !x.Attributes.Any(a => a is DummyCommandAttribute)).ToListAsync())
                    .Select(x => FormatCommandShort(x, false))
                    .Join(", "));
            }
            else
            {
                if (command.Remarks != null)
                    embed.AppendDescription($" {command.Remarks}");

                if (!command.FullAliases.IsEmpty())
                    embed.AddField("Aliases", command.FullAliases.Select(x => Format.Code(x)).Join(", "), true);

                if (!command.Parameters.IsEmpty())
                    embed.AddField("Parameters", command.Parameters.Select(FormatParameter).Join("\n"));

                if (command.CustomArgumentParserType is null)
                    embed.AddField("Usage", FormatUsage(ctx, command));

                if (command.Attributes.Any(x => x is ShowPlaceholdersInHelpAttribute))
                    embed.AddField("Placeholders",
                        WelcomeOptions.ValidPlaceholders
                            .Select(x => $"{Format.Code($"{{{x.Key}}}")}: {Format.Italics(x.Value)}")
                            .Join("\n"));

                if (command.Attributes.Any(x => x is ShowTimeFormatInHelpAttribute))
                    embed.AddField("Example Valid Time",
                        $"{Format.Code("4d3h2m1s")}: {Format.Italics("4 days, 3 hours, 2 minutes and one second.")}");

                if (command.Attributes.AnyGet(x => x is ShowUnixArgumentsInHelpAttribute, out var unixAttr) && unixAttr is ShowUnixArgumentsInHelpAttribute attr)
                {
                    static string FormatUnixArgs(KeyValuePair<string[], string> kvp) =>
                        $"{Format.Bold(kvp.Key.Select(name => $"-{name}").Join(" or "))}: {kvp.Value}";

                    static string GetArgs(VolteUnixCommand unixCommand) => unixCommand switch
                    {
                        VolteUnixCommand.Announce => AdminUtilityModule.AnnounceNamedArguments.Select(FormatUnixArgs).Join("\n"),
                        VolteUnixCommand.Zalgo => UtilityModule.ZalgoNamedArguments.Select(FormatUnixArgs).Join("\n"),
                        VolteUnixCommand.UnixBan => ModerationModule.UnixBanNamedArguments.Select(FormatUnixArgs).Join("\n"),
                        _ => throw new ArgumentOutOfRangeException(nameof(unixCommand))
                    };

                    embed.AddField("Unix Arguments", GetArgs(attr.VolteUnixCommand));
                }
            }

            var checks = CommandUtilities.EnumerateAllChecks(command).ToList();
            return !checks.IsEmpty()
                ? embed.AddField("Checks",
                    (await Task.WhenAll(checks.Select(check => FormatCheckAsync(check, ctx)))).Join("\n"))
                : embed;
        }

        public static string FormatUsage(VolteContext ctx, Command cmd)
        {
            static string FormatUsageParameter(Parameter param)
                => new StringBuilder(param.IsOptional ? "[" : "{")
                    .Append(param.Name)
                    .Append(param.IsOptional ? "]" : "}")
                    .ToString();

            return new StringBuilder($"{ctx.GuildData.Configuration.CommandPrefix}{cmd.FullAliases.First().ToLower()} ")
                .Append(cmd.Parameters.Select(FormatUsageParameter).Join(" "))
                .ToString().Trim();
        }

        private static async Task<string> FormatCheckAsync(CheckAttribute cba, VolteContext context)
        {
            var result = await cba.CheckAsync(context);
            var message = GetCheckFriendlyMessage(context, cba);
            return $"- {(result.IsSuccessful ? DiscordHelper.BallotBoxWithCheck : DiscordHelper.X)} {message}";
        }

        private static string GetCheckFriendlyMessage(VolteContext ctx, CheckAttribute cba)
            => cba switch
            {
                RequireBotChannelPermissionAttribute rbcp =>
                    $"I require the channel permission(s) {rbcp.Permissions.Select(x => x.ToString().Humanize(LetterCasing.Title)).Humanize()}.",
                RequireBotGuildPermissionAttribute rbgp =>
                    $"I require the guild permission(s) {rbgp.Permissions.Select(x => x.ToString().Humanize(LetterCasing.Title)).Humanize()}.",
                RequireGuildAdminAttribute _ => "You need to have the Admin role.",
                RequireGuildModeratorAttribute _ => "You need to have the Moderator role.",
                RequireBotOwnerAttribute _ => $"Only usable by **{ctx.Client.GetOwner()}** (bot owner).",
                _ => $"Unimplemented check: {cba.GetType().AsPrettyString()}. Please report this to my developers :)"
            };

        private static string FormatParameter(Parameter param)
            => new StringBuilder(Format.Code(param.Name)).Apply(sb =>
            {
                if (!param.Description.IsNullOrWhitespace())
                    sb.Append($": {param.Description} ");
                if (param.Checks.Any(x => x is EnsureNotSelfAttribute))
                    sb.Append("Cannot be yourself.");
                if (param.DefaultValue != null)
                    sb.Append($"Defaults to: {Format.Code(param.DefaultValue.ToString())}");
            }).ToString().Trim();

        public static string SanitizeName(this Module m)
            => m.Name.Replace("Module", string.Empty);

        public static string SanitizeParserName(this Type type)
            => type.Name.Replace("Parser", string.Empty);

        internal static IEnumerable<Type> AddTypeParsers(this CommandService service)
        {
            var assembly = typeof(VolteBot).Assembly;
            var parsers = assembly.ExportedTypes.Where(x => x.HasAttribute<InjectTypeParserAttribute>()).ToList();

            foreach (var parser in parsers)
            {
                var parserObj = parser.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
                var method = typeof(CommandService).GetMethod("AddTypeParser")?.MakeGenericMethod(
                    parser.BaseType?.GenericTypeArguments[0]
                    ?? throw new FormatException("CommandService#AddTypeParser() values invalid."));
                // ReSharper disable twice PossibleNullReferenceException
                // cant happen
                method.Invoke(service,
                    new[] {parserObj, parser.GetCustomAttribute<InjectTypeParserAttribute>().OverridePrimitive});
                yield return parser;
            }
        }

        public static Command GetCommand(this CommandService service, string name)
            => service.FindCommands(name).FirstOrDefault()?.Command;

        public static int GetTotalTypeParsers(this CommandService cs)
        {
            var customParsers = typeof(VolteBot).Assembly.GetTypes()
                .Count(x => x.HasAttribute<InjectTypeParserAttribute>());
            //add the number of primitive TypeParsers (that come with Qmmands) obtained from the private field _primitiveTypeParsers's Count
            // ReSharper disable twice PossibleNullReferenceException
            var primitiveTypeParsers = cs.GetType()
                .GetField("_primitiveTypeParsers", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(cs)
                .Cast<IDictionary>();
            return customParsers + primitiveTypeParsers.Count;
        }
    }
}