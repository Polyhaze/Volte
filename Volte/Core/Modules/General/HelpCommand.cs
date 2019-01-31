using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Extensions;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Modules.General {
    public partial class GeneralModule : VolteModule {
        public CommandService Cs { get; set; }

        [Command("Help", RunMode = RunMode.Async), Alias("H")]
        [Summary("Shows Volte's Modules and Commands.")]
        public async Task Help(string mdl = null) {
            var config = Db.GetConfig(Context.Guild);
            var modules = Cs.Modules.OrderBy(m => m.Name);
            var embed = new EmbedBuilder()
                .WithColor(Config.GetSuccessColor())
                .WithAuthor(Context.User);
            var desc = string.Empty;
            if (mdl == null) {
                embed.WithTitle("Available Modules");
                foreach (var module in modules) {
                    desc += $"**{module.Name.Replace("Module", string.Empty)}**\n\n";
                }

                embed.WithDescription(desc);
                await Reply(Context.Channel, embed.Build());
                return;
            }

            if (mdl.StartsWith(config.CommandPrefix)) {
                var cname = mdl.Replace(config.CommandPrefix, string.Empty);
                var c = Cs.Commands.FirstOrDefault(x => x.Name.EqualsIgnoreCase(cname));

                if (c == null) {
                    await Reply(Context.Channel, CreateEmbed(Context, "No command matching that name was found."));
                    return;
                }

                var aliases = c.Aliases.Aggregate("(", (current, alias) => current + alias + "|");

                if ((c.Module.Name.EqualsIgnoreCase("adminmodule") && !UserUtils.IsAdmin(Context)) || 
                    (c.Module.Name.EqualsIgnoreCase("ownermodule") && !UserUtils.IsBotOwner(Context.User)) || 
                    (c.Module.Name.EqualsIgnoreCase("moderationmodule") && !UserUtils.IsModerator(Context))) {
                    await Reply(Context.Channel,
                        CreateEmbed(Context, "You don't have permission to use the module that command is from."));
                    return;
                }

                aliases += ")";

                embed.WithDescription($"**Command**: {c.Name}\n" +
                                      $"**Module**: {c.Module.Name.Replace("Module", string.Empty)}\n" +
                                      $"**Summary**: {c.Summary ?? "No summary provided."}\n" +
                                      "**Usage**: " + c.Remarks
                                          .Replace(c.Name.ToLower(), aliases.Replace("|)", ")"))
                                          .Replace("|prefix|", config.CommandPrefix)
                                          .Replace("Usage: ", string.Empty));
                await Reply(Context.Channel, embed.Build());
                return;
            }

            var target = Cs.Modules.FirstOrDefault(x => x.Name.Replace("Module", string.Empty).EqualsIgnoreCase(mdl));
            if (target == null) {
                await Reply(Context.Channel, CreateEmbed(Context, "Specified module not found."));
                return;
            }

            if ((mdl.EqualsIgnoreCase("admin") && !UserUtils.IsAdmin(Context)) || 
                (mdl.EqualsIgnoreCase("owner") && !UserUtils.IsBotOwner(Context.User)) || 
                (mdl.EqualsIgnoreCase("moderation") && !UserUtils.IsModerator(Context))) {
                await Reply(Context.Channel,
                    CreateEmbed(Context, "You don't have permission to use the module that command is from."));
                return;
            }

            embed.WithTitle($"Commands for {target.Name.Replace("Module", string.Empty)}");
            desc = target.Commands.Aggregate(desc, (current, cmd)
                => current + $"**{cmd.Name}**: `{cmd.Summary ?? "No summary specified"}`\n\n");
            embed.WithDescription(desc);

            await Reply(Context.Channel, embed.Build());
        }
    }
}