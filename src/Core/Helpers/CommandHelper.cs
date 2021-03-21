using System.Linq;
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
        public static async Task<bool> CanShowCommandAsync(VolteContext ctx, Command command) => await command.RunChecksAsync(ctx) is SuccessfulResult;

        public static async Task<bool> CanShowModuleAsync(VolteContext ctx, Module module) => await module.RunChecksAsync(ctx) is SuccessfulResult;


        public static string FormatCommandShort(Command command, bool includeGroup = true)
        {
            var firstAlias = command.FullAliases.FirstOrDefault();
            if (firstAlias is null)
                return null;

            if (includeGroup)
            {
                return Format.Bold($"`{firstAlias}`");
            }
            
            return Format.Bold($"`{firstAlias.Split(" ")[1]}`");
        }

        public static async Task<EmbedBuilder> CreateCommandEmbedAsync(Command command, VolteContext ctx)
        {
            var embed = ctx.CreateEmbedBuilder()
                .WithTitle("Command information")
                .WithDescription(
                    $"{Format.Code(command.FullAliases.First())}: {command.Description ?? "No description provided."}");
            
            if (command.Attributes.Any(x => x is DummyCommandAttribute))
            {
                embed.AddField("Subcommands", command.Module.Commands
                    .Where(x => !x.Attributes.Any(a => a is DummyCommandAttribute))
                    .Select(x => FormatCommandShort(x, false))
                    .Join(", "));
            }
            else
            {
                if (command.Remarks != null) embed.Description += " " + command.Remarks;

                if (command.FullAliases.Count > 1)
                    embed.AddField("Aliases", command.FullAliases.Skip(1).Join(", "), true);

                if (command.Parameters.Count > 0)
                {
                    embed.AddField("Parameters",
                        command.Parameters.Select(FormatParameter).Join("\n"));
                }

                if (command.CustomArgumentParserType == null)
                {
                    var cExecString = $"{ctx.GuildData.Configuration.CommandPrefix}{command.FullAliases.First().ToLower()} " +
                                      $"{command.Parameters.Select(a => $"{(a.IsOptional ? "[" : "{")}{a.Name}{(a.IsOptional ? "]" : "}")}").Join(" ")}";
                    embed.AddField("Usage", cExecString);
                }
            }

            var checks = CommandUtilities.EnumerateAllChecks(command).ToList();
            if (!checks.IsEmpty())
            {
                embed.AddField("Checks", (await Task.WhenAll(checks.Select(check => FormatCheckAsync(check, ctx)))).Join("\n"));
            }

            return embed;
        }

        private static async Task<string> FormatCheckAsync(CheckAttribute cba, VolteContext context)
        {
            var result = await cba.CheckAsync(context);
            var message = await GetCheckFriendlyMessage(context, cba);
            return $"- {(result.IsSuccessful ? ":white_check_mark:" : ":red_circle:")} {message}";
        }

        private static async Task<string> GetCheckFriendlyMessage(VolteContext ctx, CheckAttribute cba)
        {
            return cba switch
            {
                RequireBotChannelPermissionAttribute rbcp =>
                    $"I require the guild permission(s) {rbcp.Permissions.Humanize()}.",
                RequireBotGuildPermissionAttribute rbgp =>
                    $"I require the channel permission {rbgp.Permissions.Humanize()}.",
                RequireGuildAdminAttribute _ => "You need the to have the Admin role for this guild.",
                RequireGuildModeratorAttribute _ => "You need the to have the Moderator role for this guild.",
                RequireBotOwnerAttribute _ =>
                    $"Only usable by **{await ctx.Client.Rest.GetUserAsync(Config.Owner)}** (bot owner).",
                _ => $"Unimplemented check: {cba.GetType().AsPrettyString()}"
            };
        }

        private static string FormatParameter(Parameter parameterInfo)
        {
            return !parameterInfo.Description.IsNullOrWhitespace() ? $"`{parameterInfo.Name}`: {parameterInfo.Description}" : $"`{parameterInfo.Name}`";
        }
    }
}