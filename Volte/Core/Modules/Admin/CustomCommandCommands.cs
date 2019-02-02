using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("CustomCommandAdd"), Alias("Cca")]
        [Summary("Add a custom command for this guild.")]
        [Remarks("Usage: |prefix|customcommandadd {cmdName} {cmdResponse}")]
        public async Task CustomCommandAdd(string name, [Remainder] string response) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.ReactFailure();
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.CustomCommands.Add(name, response);
            Db.UpdateConfig(config);
            await Context.Channel.SendMessageAsync(string.Empty, false,
                CreateEmbed(Context, string.Empty)
                    .ToEmbedBuilder()
                    .AddField("Command Name", name)
                    .AddField("Command Response", response)
                    .Build()
            );
        }
        
        [Command("CustomCommandRem"), Alias("Ccr")]
        [Summary("Remove a custom command from this guild.")]
        [Remarks("Usage: |prefix|customcommandrem {cmdName}")]
        public async Task CustomCommandRem(string cmdName) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.ReactFailure();
                return;
            }
            var config = Db.GetConfig(Context.Guild);
            var embed = CreateEmbed(Context, string.Empty).ToEmbedBuilder()
                .WithAuthor(Context.User);
            
            if (config.CustomCommands.Keys.Contains(cmdName)) {
                config.CustomCommands.Remove(cmdName);
                Db.UpdateConfig(config);
                embed.WithDescription($"Removed **{cmdName}** from this server's Custom Commands.");
            }
            else {
                embed.WithDescription($"**{cmdName}** is not a command on this server.");
            }

            await Context.Channel.SendMessageAsync(string.Empty, false, embed.Build());
        }

        [Command("CustomCommandClear"), Alias("Ccc")]
        [Summary("Clears the custom commands for this guild.")]
        [Remarks("Usage: |prefix|customcommandclear")]
        public async Task CustomCommandClear() {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.ReactFailure();
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            await Context.Channel.SendMessageAsync(string.Empty, false,
                CreateEmbed(Context,
                    $"Cleared the custom commands, containing **{config.CustomCommands.Count}** words."));
            config.CustomCommands.Clear();
            Db.UpdateConfig(config);
        }
    }
}