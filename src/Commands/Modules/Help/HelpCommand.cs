using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Help
{
    public partial class HelpModule : VolteModule
    {
        [Command("Help", "H")]
        [Description("Shows the commands used for module listing, command listing, and command info.")]
        [Remarks("Usage: |prefix|help")]
        public async Task HelpAsync()
        {
            var data = Db.GetData(Context.Guild);
            var embed = Context.CreateEmbedBuilder()
                .WithDescription("Hey, I'm Volte! Here's a list of my commands designed to help you out. " +
                                 $"If you're new here, try out `{data.Configuration.CommandPrefix}mdls` to list all of my modules!" +
                                 "\n\n" +
                                 "{} = required argument | [] = optional argument" +
                                 "\n   \n" +
                                 $"**List Modules**: {data.Configuration.CommandPrefix}mdls" +
                                 "\n" +
                                 $"**List Commands in a Module**: {data.Configuration.CommandPrefix}cmds {{moduleName}}" +
                                 "\n" +
                                 $"**Show Info about a Command**: {data.Configuration.CommandPrefix}cmd {{commandName}}");

            await embed.SendToAsync(Context.Channel);
        }
    }
}