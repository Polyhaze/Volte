using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Commands.Modules
{
    [Group("Settings", "Setting", "Options", "Option")]
    [RequireGuildAdmin]
    public partial class SettingsModule : VolteModule
    {
        public WelcomeService WelcomeService { get; set; }

        [Command, DummyCommand]
        public Task<ActionResult> BaseAsync() => None();
    }
}