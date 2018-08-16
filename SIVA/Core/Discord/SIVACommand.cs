using Discord;
using Discord.Commands;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord {
    
    public abstract class SIVACommand : ModuleBase<SocketCommandContext> {
        public static Log GetLogger() => new Log();
    }
}