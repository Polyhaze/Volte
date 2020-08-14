using Qmmands;
using Volte.Core.Attributes;
using Volte.Services;

namespace Volte.Commands.Modules
{
    [Group("Welcome", "W")]
    [RequireGuildAdmin]
    public sealed partial class WelcomeModule : VolteModule
    {
        public WelcomeService WelcomeService { get; set; }
    }
}