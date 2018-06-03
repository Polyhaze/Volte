using Discord;
using Discord.Commands;

namespace SIVA.Core.Modules
{
    public abstract class SivaModule : ModuleBase<SocketCommandContext>
    {
        public bool Disabled = false;
        public bool Enabled = true;
        public int Zero = 0;
    }
}