using Discord.Commands;

using BrackeysBot.Services;

namespace BrackeysBot.Commands
{
    [Name("Custom Commands")]
    [ModuleColor(0xafc750)]
    [Summary("Provides customizeable commands!")]
    public partial class CustomCommandsModule : BrackeysBotModule
    {
        public CustomCommandService CustomCommands { get; set; }
    }
}