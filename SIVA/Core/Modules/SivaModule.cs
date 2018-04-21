using Discord.Commands;
using SIVA.Core.Bot;
using SIVA.Core.Bot.Services;

namespace SIVA.Core.Modules
{
    public abstract class SivaModule : ModuleBase<SocketCommandContext>
    {
        private EventHandler _eHandler;
    }
    
    public class SivaModule<TService> : SivaModule where TService : INService
    {
        //public SocketCommandContext Context;
        public TService _service { get; set; }
    }
}