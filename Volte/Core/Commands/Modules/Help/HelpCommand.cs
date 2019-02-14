using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Help
{
    public partial class HelpModule : VolteModule
    {
        [Command("Help")]
        [Description("Shows the commands used for module listing, command listing, and command info.")]
        [Remarks("Usage: |prefix|help")]
        public async Task Help()
        {
            var config = Db.GetConfig(Context.Guild);
            var embed = Context.CreateEmbed(string.Empty).ToEmbedBuilder()
                .AddField("Modules", $"List modules.\nUsage: {config.CommandPrefix}mdls", true)
                .AddField("Commands", $"Shows commands in a module.\nUsage: {config.CommandPrefix}cmds {{moduleName}}",
                    true)
                .AddField("Command", $"Shows info about a command.\nUsage: {config.CommandPrefix}cmd {{cmdName}}",
                    true);

            await embed.SendTo(Context.Channel);
        }
    }
}