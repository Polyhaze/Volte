using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Help
{
    public partial class HelpModule : VolteModule
    {
        [Command("Help", "H")]
        [Description("Shows the commands used for module listing, command listing, and command info.")]
        [Remarks("Usage: |prefix|help")]
        public async Task Help()
        {
            var config = Db.GetConfig(Context.Guild);
            var embed = Context.CreateEmbed(string.Empty).ToEmbedBuilder()
                .WithDescription("Hey, I'm Volte! Here's a list of my commands designed to help you out! " +
                                 $"If you're new here, try out `{config.CommandPrefix}mdls` to list all of my modules!" +
                                 "\n\n" +
                                 "{} = required argument | [] = optional argument" +
                                 "\n   \n" +
                                 $"**List Modules**: {config.CommandPrefix}mdls" +
                                 "\n" +
                                 $"**List Commands in a Module**: {config.CommandPrefix}cmds {{moduleName}}" +
                                 "\n" +
                                 $"**Show Info about a Command**: {config.CommandPrefix}cmd {{commandName}}");

            await embed.SendTo(Context.Channel);
        }
    }
}