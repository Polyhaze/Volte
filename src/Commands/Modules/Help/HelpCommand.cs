using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using BrackeysBot.Services;

namespace BrackeysBot.Commands
{
    [HideFromHelp]
    public sealed class HelpModule : BrackeysBotModule
    {
        public CommandService Commands { get; set; }
        public ModuleService Modules { get; set; }
        public IServiceProvider Provider { get; set; }

        private const string _noDescription = "No description provided.";
        private const string _noUsage = "No usage provided.";

        [Command("help")]
        [Summary("Displays a list of modules.")]
        public async Task HelpAsync()
        {
            IEnumerable<ModuleInfo> availableModules = Commands.Modules
                .Where(module => !module.HasAttribute<HideFromHelpAttribute>()
                    && module.Commands.Any(c => c.CheckPreconditionsAsync(Context).GetAwaiter().GetResult().IsSuccess));
            IEnumerable<EmbedFieldBuilder> fields = availableModules
                .Select(module => new EmbedFieldBuilder().WithName(module.Name.Sanitize().Envelop("**")).WithValue(module.Summary.WithAlternative("No description provided.")));

            await GetDefaultBuilder()
                .WithDescription($"Here's a list of modules you can access!\nType `{ExtractPrefixFromContext(Context)}help <module>` to see all the commands of that module!")
                .WithFields(fields.ToArray())
                .Build()
                .SendToChannel(Context.Channel);
        }

        [Command("help")]
        [Summary("Displays more information about a module or command.")]
        public async Task HelpAsync([Remainder] string identifier)
        {
            CommandInfo commandInfo = GetTargetCommand(identifier);
            ModuleInfo moduleInfo = GetTargetModule(identifier);

            if (commandInfo == null && moduleInfo == null)
            {
                await ReplyAsync($"A command or module with the name **{identifier}** could not be found.");
            }

            if (commandInfo != null && moduleInfo == null)
            {
                await DisplayCommandHelpAsync(commandInfo, Context);
            }
            if (moduleInfo != null)
            {
                await DisplayModuleHelpAsync(moduleInfo, Context);
            }
        }

        public static async Task DisplayCommandHelpAsync(CommandInfo command, ICommandContext context)
        {
            string title = command.Name;
            if (command.Aliases.Count > 1)
                title += $" ({string.Join('|', command.Aliases)})";

            string prefix = ExtractPrefixFromContext(context);

            StringBuilder description = new StringBuilder()
                .AppendLine(command.Summary.WithAlternative(_noDescription))
                .AppendLine()
                .AppendLine("**Module**: " + command.Module.Name.Sanitize())
                .AppendLine("**Usage**: " + (string.IsNullOrEmpty(command.Remarks) ? _noUsage : prefix + command.Remarks));

            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description.ToString())
                .WithColor(command.GetColor())
                .WithFields(command.Parameters.Select(InfoToEmbedField));

            await builder.Build().SendToChannel(context.Channel);
        }
        public static async Task DisplayModuleHelpAsync(ModuleInfo module, ICommandContext context)
        {
            string prefix = ExtractPrefixFromContext(context);

            IEnumerable<CommandInfo> displayable = module.Commands
                .Where(c => c.CheckPreconditionsAsync(context).GetAwaiter().GetResult().IsSuccess
                    && !c.HasAttribute<HideFromHelpAttribute>());

            bool displayModuleHelp = displayable.Count() > 0 && !module.HasAttribute<HideFromHelpAttribute>();
            if (displayModuleHelp)
            {
                EmbedBuilder builder = new EmbedBuilder()
                    .WithTitle(module.Name.Sanitize())
                    .WithColor(module.GetColor())
                    .WithFields(displayable.Select(c => InfoToEmbedField(c, prefix)));

                if (!string.IsNullOrEmpty(module.Summary))
                    builder.WithDescription(module.Summary);

                await builder.Build()
                    .SendToChannel(context.Channel);
            }
        }

        private CommandInfo GetTargetCommand(string name)
            => Commands.Commands
                .FirstOrDefault(c => c.Aliases.Any(a => string.Equals(name, a, StringComparison.InvariantCultureIgnoreCase)));
        private ModuleInfo GetTargetModule(string name)
            => Commands.Modules
                .FirstOrDefault(m => !m.IsSubmodule && string.Equals(name, m.Name.Sanitize(), StringComparison.InvariantCultureIgnoreCase));

        private static string ExtractPrefixFromContext(ICommandContext context)
            => (context as BrackeysBotContext)?.Configuration.Prefix ?? string.Empty;

        private static EmbedFieldBuilder InfoToEmbedField(ParameterInfo info)
            => new EmbedFieldBuilder()
                .WithName(info.Name)
                .WithValue(info.Summary.WithAlternative(_noDescription))
                .WithIsInline(true);
        private static EmbedFieldBuilder InfoToEmbedField(CommandInfo info, string prefix)
            => new EmbedFieldBuilder()
                .WithName(string.Concat(prefix, info.Remarks.WithAlternative(info.Name)))
                .WithValue(info.Summary.WithAlternative(_noDescription))
                .WithIsInline(false);
    }
}
