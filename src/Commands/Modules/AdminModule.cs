using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Commands.Modules
{
    [RequireGuildAdmin]
    public sealed partial class AdminModule : VolteModule
    {
        public WelcomeService WelcomeService { get; set; }
    }
}