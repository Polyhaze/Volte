using System;
using System.Threading.Tasks;

namespace Volte.Services
{
    public abstract class VolteEventService
    {
        public abstract Task DoAsync(EventArgs args);
    }
}
