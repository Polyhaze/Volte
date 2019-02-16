using System.Diagnostics;
using Discord;
using Volte.Core.Services;
using Qmmands;

namespace Volte.Core.Commands
{
    public abstract class VolteModule : ModuleBase<VolteContext>
    {
        protected DatabaseService Db { get; set; }
        protected DebugService DebugService { get; set; }
        protected CommandService CommandService { get; set; }
        protected EmojiService EmojiService { get; set; }
        protected LoggingService Logger { get; set; }
    }
}