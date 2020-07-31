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
        [Command("ccfeature"), Alias("ccf")]
        [Summary("Adds a feature to a command.")]
        [Remarks("ccfeature <command> <feature> <arguments>")]
        [RequireModerator]
        public async Task AddFeatureAsync(
            [Summary("The name of the command.")] string command,
            [Summary("The name of the feature.")] string feature,
            [Summary("The arguments to pass to the feature."), Remainder] string arguments)
        {
            if (!CustomCommands.TryGetCommand(command, out CustomCommand customCommand))
                throw new InvalidOperationException($"A command with the name `{command}` does not exist.");

            var commandFeature = CustomCommands.CreateFeature(feature, arguments);

            // Ensure no duplicate features exist
            int previousFeatures = customCommand.Features.RemoveAll(f => f.GetType().Equals(commandFeature.GetType()));
            // Add the new feature
            customCommand.Features.Add(commandFeature);

            CustomCommands.SaveCustomCommands();

            await GetDefaultBuilder()
                .WithColor(Color.Green)
                .WithDescription(previousFeatures == 0
                    ? $"The feature `{feature}` was added to `{command}`!"
                    : $"The feature `{feature}` was updated in `{command}`!")
                .Build()
                .SendToChannel(Context.Channel);
        }

        [Command("ccdelfeature"), Alias("ccdf", "ccrf")]
        [Summary("Removes a feature from a command.")]
        [Remarks("ccdelfeature <command> <feature>")]
        [RequireModerator]
        public async Task RemoveFeatureAsync(
            [Summary("The name of the command.")] string command,
            [Summary("The name of the feature.")] string feature)
        {
            if (!CustomCommands.TryGetCommand(command, out CustomCommand customCommand))
                throw new InvalidOperationException($"A command with the name `{command}` does not exist.");

            CustomCommands.RemoveFeatureFromCommand(customCommand, feature);

            await GetDefaultBuilder()
                .WithDescription($"The feature `{feature}` has been removed from `{command}`.")
                .Build()
                .SendToChannel(Context.Channel);
        }

        [Command("ccfeatures"), Alias("ccfs")]
        [Summary("Displays a list of available command features.")]
        [RequireModerator]
        public async Task ShowFeaturesAsync()
        {
            await GetDefaultBuilder()
                .WithDescription("Here's a list of available features!")
                .WithFields(CustomCommands.GetFeatureInfos()
                    .Select(f => new EmbedFieldBuilder().WithName(f.Name).WithValue(f.Summary.WithAlternative("No description provided."))))
                .Build()
                .SendToChannel(Context.Channel);
        }
    }
}
