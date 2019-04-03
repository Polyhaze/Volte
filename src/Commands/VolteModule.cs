using Volte.Services;
using Qmmands;

namespace Volte.Commands
{
    public abstract class VolteModule : ModuleBase<VolteContext>
    {
        public DatabaseService Db { get; set; }
        public EventService EventService { get; set; }
        public DebugService DebugService { get; set; }
        public CommandService CommandService { get; set; }
        public EmojiService EmojiService { get; set; }
        public LoggingService Logger { get; set; }
    }
}