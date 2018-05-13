using Discord.Commands;

namespace SIVA.Core.Modules
{
    public abstract class SivaModule : ModuleBase<SocketCommandContext>
    {
        public bool Enabled = true;
        public bool Disabled = false;
        public int Zero = 0;
    }
}
