using System;
using System.Threading.Tasks;

namespace Volte.Services
{
    public abstract class VolteEventService<T> where T : EventArgs
    {
        public abstract Task DoAsync(T args);
    }
}
