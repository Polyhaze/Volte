using Volte.Services;
using Qmmands;

namespace Volte.Commands
{
    public class VolteModule : ModuleBase<VolteContext>
    {
        protected DatabaseService Db { get; set; }
        protected DebugService DebugService { get; set; }
        protected CommandService CommandService { get; set; }
        protected EmojiService EmojiService { get; set; }
        protected LoggingService Logger { get; set; }
    }
}