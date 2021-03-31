using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands;
using Volte.Core.Entities;

namespace Volte.Core.Helpers
{
    public static class CommandHelper
    {
        public static async Task<bool> CanShowCommandAsync(VolteContext ctx, Command command) =>
            await command.RunChecksAsync(ctx) is SuccessfulResult;

        public static async Task<bool> CanShowModuleAsync(VolteContext ctx, Module module) =>
            await module.RunChecksAsync(ctx) is SuccessfulResult;


        public static string FormatCommandShort(Command command, bool includeGroup = true)
        {
            var firstAlias = command.FullAliases.FirstOrDefault();
            if (firstAlias is null) return null;

            if (!firstAlias.Contains(command.Service.Separator)) return $"`{firstAlias}`";

            return includeGroup
                ? Format.Bold($"`{firstAlias}`")
                : Format.Bold($"`{firstAlias.Split(command.Service.Separator)[1]}`");
        }

        public static string FormatModuleShort(Module module)
        {
            var firstAlias = module.FullAliases.FirstOrDefault();
            if (firstAlias is null) return null;

            return Format.Bold($"`{firstAlias}`");
        }

        public static async Task<EmbedBuilder> CreateCommandEmbedAsync(Command command, VolteContext ctx)
        {
            var embed = ctx.CreateEmbedBuilder()
                .WithTitle(command.Name)
                .WithDescription($"{command.Description ?? "No description provided."}");

            if (command.Attributes.Any(x => x is DummyCommandAttribute))
            {
                embed.AddField("Subcommands", command.Module.Commands
                    .Where(x => !x.Attributes.Any(a => a is DummyCommandAttribute))
                    .Where(x => x.RunChecksAsync(ctx).Result.IsSuccessful) //yes; i know .Result is bad, however linq's Where() doesn't let you use async as its an invalid return type.
                    .Select(x => FormatCommandShort(x, false))
                    .Join(", "));
            }
            else
            {
                if (command.Remarks != null)
                    embed.Description += " " + command.Remarks;

                if (command.FullAliases.Count > 1)
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
            }

            var checks = CommandUtilities.EnumerateAllChecks(command).ToList();
            if (!checks.IsEmpty())
                embed.AddField("Checks",
                    (await Task.WhenAll(checks.Select(check => FormatCheckAsync(check, ctx)))).Join("\n"));

            return embed;
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
                .ToString();
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
                RequireGuildAdminAttribute _ => "You need the to have the Admin role.",
                RequireGuildModeratorAttribute _ => "You need the to have the Moderator role.",
                RequireBotOwnerAttribute _ =>
                    $"Only usable by **{ctx.Client.GetOwner()}** (bot owner).",
                _ => $"Unimplemented check: {cba.GetType().AsPrettyString()}. Please report this to my developers :)"
            };

        private static string FormatParameter(Parameter param)
        {
            var sb = new StringBuilder(Format.Code(param.Name));
            if (!param.Description.IsNullOrWhitespace())
                sb.Append($": {param.Description} ");
            if (param.Checks.Any(x => x is EnsureNotSelfAttribute))
                sb.Append("Cannot be yourself.");
            if (param.DefaultValue != null)
                sb.Append($"Defaults to: `{param.DefaultValue}`");

            return sb.ToString().Trim();
        }
    }
}