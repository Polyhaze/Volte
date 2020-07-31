using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using BrackeysBot.Core.Models;

namespace BrackeysBot.Commands
{
    public partial class CustomCommandsModule : BrackeysBotModule
    {
        [Command("ccadd"), Alias("cc", "customcommand")]
        [Summary("Creates a new customizeable command.")]
        [Remarks("ccadd <name>")]
        [RequireModerator]
        public async Task CreateCommandAsync(
            [Summary("The name of the command.")] string name)
        {
            if (CustomCommands.ContainsCommand(name, new string[0]))
                throw new InvalidOperationException($"A command with the name `{name}` already exists!");

            CustomCommands.CreateCommand(name);

            await GetDefaultBuilder()
                .WithColor(Color.Green)
                .WithDescription($"The command `{name}` was added!\n\nUse `ccfeature` to add features to the command!")
                .Build()
                .SendToChannel(Context.Channel);
        }

        [Command("ccdel"), Alias("ccdelete", "ccremove")]
        [Summary("Deletes a custom command.")]
        [Remarks("ccdel <name>")]
        [RequireModerator]
        public async Task RemoveCommandAsync(
            [Summary("The name of the command.")] string name)
        {
            if (!CustomCommands.ContainsCommand(name, new string[0]))
                throw new InvalidOperationException($"A command with the name `{name}` does not exist.");

            CustomCommands.RemoveCommand(name);

            await GetDefaultBuilder()
                .WithColor(Color.Green)
                .WithDescription($"The command `{name}` was removed.")
                .Build()
                .SendToChannel(Context.Channel);
        }

        [Command("ccinfo"), Alias("cci")]
        [Summary("Displays information about a custom command.")]
        [Remarks("ccinfo <name>")]
        [RequireModerator]
        public async Task ShowCommandInfoAsync(
            [Summary("The name of the command.")] string name)
        {
            if (!CustomCommands.TryGetCommand(name, out CustomCommand command))
                throw new InvalidOperationException($"A command with the name `{name}` does not exist.");

            await GetDefaultBuilder()
                .WithTitle(command.Name)
                .WithFields(command.Features.Select(f => new EmbedFieldBuilder()
                    .WithName(f.Name)
                    .WithValue(f.ToString())))
                .Build()
                .SendToChannel(Context.Channel);
        }
    }
}
