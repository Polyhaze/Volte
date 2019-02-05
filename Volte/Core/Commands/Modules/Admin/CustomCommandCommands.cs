using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Data;
using Volte.Core.Extensions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("CustomCommandAdd"), Alias("Cca")]
        [Summary("Add a custom command for this guild.")]
        [Remarks("Usage: |prefix|customcommandadd {cmdName} {cmdResponse}")]
        [RequireGuildAdmin]
        public async Task CustomCommandAdd(string name, [Remainder] string response) {
            var config = Db.GetConfig(Context.Guild);
            config.CustomCommands.Add(name, response);
            Db.UpdateConfig(config);
            await Context.CreateEmbed(string.Empty)
                .ToEmbedBuilder()
                .AddField("Command Name", name)
                .AddField("Command Response", response)
                .SendTo(Context.Channel);
        }

        [Command("CustomCommandRem"), Alias("Ccr")]
        [Summary("Remove a custom command from this guild.")]
        [Remarks("Usage: |prefix|customcommandrem {cmdName}")]
        [RequireGuildAdmin]
        public async Task CustomCommandRem(string cmdName) {
            var config = Db.GetConfig(Context.Guild);
            var embed = Context.CreateEmbed(string.Empty).ToEmbedBuilder()
                .WithAuthor(Context.User);

            if (config.CustomCommands.Keys.Contains(cmdName)) {
                config.CustomCommands.Remove(cmdName);
                Db.UpdateConfig(config);
                embed.WithDescription($"Removed **{cmdName}** from this server's Custom Commands.");
            }
            else {
                embed.WithDescription($"**{cmdName}** is not a command on this server.");
            }

            await embed.SendTo(Context.Channel);
        }

        [Command("CustomCommandClear"), Alias("Ccc")]
        [Summary("Clears the custom commands for this guild.")]
        [Remarks("Usage: |prefix|customcommandclear")]
        [RequireGuildAdmin]
        public async Task CustomCommandClear() {
            var config = Db.GetConfig(Context.Guild);
            await Context
                .CreateEmbed($"Cleared the custom commands, containing **{config.CustomCommands.Count}** words.")
                .SendTo(Context.Channel);
            config.CustomCommands.Clear();
            Db.UpdateConfig(config);
        }
    }
}