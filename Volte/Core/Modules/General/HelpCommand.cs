using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Extensions;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.General {
    public partial class GeneralModule : VolteModule {
        public CommandService Cs { get; set; }

        [Command("Help", RunMode = RunMode.Async), Alias("H")]
        [Summary("Shows Volte's Modules and Commands.")]
        public async Task Help(string mdl = null) {
            var config = Db.GetConfig(Context.Guild);
            var modules = Cs.Modules.Where(x => !x.Name.Contains("Owner")).OrderBy(m => m.Name);
            var embed = new EmbedBuilder()
                .WithColor(Config.GetSuccessColor())
                .WithAuthor(Context.User);
            var desc = "";
            if (mdl == null) {
                embed.WithTitle("Available Modules");
                foreach (var module in modules) {
                    desc += $"**{module.Name.Replace("Module", "")}**\n\n";
                }

                embed.WithDescription(desc);
                await Reply(Context.Channel, embed.Build());
                return;
            }

            if (mdl.StartsWith(config.CommandPrefix)) {
                var cname = mdl.Replace(config.CommandPrefix, "");
                var c = Cs.Commands.FirstOrDefault(x => x.Name.EqualsIgnoreCase(cname));

                if (c == null) {
                    await Reply(Context.Channel, CreateEmbed(Context, "No command matching that name was found."));
                    return;
                }

                var aliases = c.Aliases.Aggregate("(", (current, alias) => current + alias + "|");
                
                if (c.Module.Name.EqualsIgnoreCase("adminmodule") && !UserUtils.IsAdmin(Context)) {
                    await Reply(Context.Channel,
                        CreateEmbed(Context, "You don't have permission to use the module that command is from."));
                    return;
                }

                if (c.Module.Name.EqualsIgnoreCase("ownermodule") && !UserUtils.IsBotOwner(Context.User)) {
                    await Reply(Context.Channel,
                        CreateEmbed(Context, "You don't have permission to use the module that command is from."));
                    return;
                }

                if (c.Module.Name.EqualsIgnoreCase("moderationmodule") && !UserUtils.IsModerator(Context)) {
                    await Reply(Context.Channel,
                        CreateEmbed(Context, "You don't have permission to use the module that command is from."));
                    return;
                }

                aliases += ")";

                embed.WithTitle($"Command {c.Name}")
                    .AddField("Module", c.Module.Name)
                    .AddField("Summary", c.Summary ?? "No summary provided.")
                    .AddField("Usage", c.Remarks
                        .Replace(c.Name.ToLower(), aliases.Replace("|)", ")"))
                        .Replace("|prefix|", config.CommandPrefix)
                        .Replace("Usage: ", ""));
                await Reply(Context.Channel, embed.Build());
                return;
            }

            var target = Cs.Modules.FirstOrDefault(x => x.Name.Replace("Module", "").EqualsIgnoreCase(mdl));
            if (target == null) {
                await Reply(Context.Channel, CreateEmbed(Context, "Specified module not found."));
                return;
            }

            if (mdl.EqualsIgnoreCase("admin") && !UserUtils.IsAdmin(Context)) {
                await Reply(Context.Channel, CreateEmbed(Context, "You don't have permission to use that module."));
                return;
            }

            if (mdl.EqualsIgnoreCase("owner") && !UserUtils.IsBotOwner(Context.User)) {
                await Reply(Context.Channel, CreateEmbed(Context, "You don't have permission to use that module."));
                return;
            }

            if (mdl.EqualsIgnoreCase("moderation") && !UserUtils.IsModerator(Context)) {
                await Reply(Context.Channel, CreateEmbed(Context, "You don't have permission to use that module."));
                return;
            }

            embed.WithTitle($"Commands for {target.Name.Replace("Module", "")}");
            foreach (var cmd in target.Commands) {
                desc += $"**{cmd.Name}**: `{cmd.Summary ?? "No summary specified"}`\n\n";
                embed.WithDescription(desc);
            }

            await Reply(Context.Channel, embed.Build());
        }
    }
}