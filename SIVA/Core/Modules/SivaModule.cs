using Discord.Commands;
using SIVA.Core.Bot.Services;

namespace SIVA.Core.Modules
{
    public abstract class SivaModule : ModuleBase<SocketCommandContext>
    {
        public bool Enabled = true;
        public bool Disabled = false;
        public int Zero = 0;
    }
    	
    public class SivaModule<TService> : SivaModule where TService : INService
    {
    }
}
