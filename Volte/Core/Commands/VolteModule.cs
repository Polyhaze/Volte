using Volte.Core.Services;
using Qmmands;

namespace Volte.Core.Commands
{
    public abstract class VolteModule : ModuleBase<VolteContext>
    {
        public DatabaseService Db { get; set; }
        public DebugService DebugService { get; set; }
        public CommandService CommandService { get; set; }
        public GitHubService GitHubService { get; set; }
        public EmojiService EmojiService { get; set; }
        public LoggingService Logger { get; set; }
    }
}