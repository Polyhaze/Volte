using Qmmands;
using Volte.Core.Commands;
using Volte.Core.Services;

namespace Volte.Core.Data.Objects
{
    public class EvalObjects
    {
        public VolteContext Context { get; set; }
        public DiscordServer Config { get; set; }
        public LoggingService Logger { get; set; }
        public CommandService CommandService { get; set; }
        public DebugService DebugService { get; set; }
        public DatabaseService DatabaseService { get; set; }
    }
}