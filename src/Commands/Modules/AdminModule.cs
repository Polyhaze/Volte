using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        public WelcomeService WelcomeService { get; set; }
    }
}