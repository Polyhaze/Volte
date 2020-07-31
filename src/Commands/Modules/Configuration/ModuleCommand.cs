using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;

using BrackeysBot.Services;

namespace BrackeysBot.Commands
{
    public partial class ConfigurationModule : BrackeysBotModule
    {
        public CommandService Service { get; set; }
        public ModuleService Modules { get; set; }

        [Command("module"), Alias("mod")]
        [Remarks("module <state> <name>")]
        [Summary("Performs an action on a module.")]
        [RequireAdministrator]
        public async Task ModifyModuleAsync(
            [Summary("The action to perform on the module.")] ModuleActionType action, 
            [Summary("The name of the module."), Remainder] string name)
        {
            if (Modules.VerifyModuleName(ref name))
            {
                bool moduleActive = Modules.GetModuleState(name);
                if (action == ModuleActionType.Status)
                {
                    await ReplyAsync($"The module **{name}** is currently **{(moduleActive ? "enabled" : "disabled")}**.");
                }
                else
                {
                    if ((action == ModuleActionType.Enable && moduleActive) ||
                        (action == ModuleActionType.Disable && !moduleActive))
                    {
                        await ReplyAsync($"The module **{name}** is already **{(moduleActive ? "enabled" : "disabled")}**!");
                    }
                    else
                    {
                        bool active = action == ModuleActionType.Enable;
                        await Modules.SetModuleState(name, active);
                        await ReplyAsync($"The module **{name}** is now **{(active ? "enabled" : "disabled")}**!");
                    }
                }
            }
            else
            {
                await ReplyAsync($"I don't know a module called **{name}**.");
            }
        }

        [Command("modules")]
        [Summary("Lists all modules.")]
        [RequireAdministrator]
        [HideFromHelp]
        public async Task ListModulesAsync()
            => await new EmbedBuilder()
                .WithTitle("Modules")
                .WithFields(CreateModuleListField("Enabled", Modules.GetEnabledModules()),
                    CreateModuleListField("Disabled", Modules.GetDisabledModules()),
                    CreateModuleListField("Essential", Modules.GetEssentialModules()))
                .WithDescription($"Use `{Data.Configuration.Prefix}module enable/disable <name>` to change the state of a module!")
                .Build()
                .SendToChannel(Context.Channel);

        private EmbedFieldBuilder CreateModuleListField(string name, string[] moduleList)
            => new EmbedFieldBuilder()
                .WithName(name)
                .WithValue(string.Join("\n", moduleList).WithAlternative("<none>"))
                .WithIsInline(true);
    }
}
