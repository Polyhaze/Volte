using System.Diagnostics;
using Volte.Commands;

namespace Volte.Core.Entities
{
    /// <summary>
    ///     The base class for all Command-related Volte EventArgs.
    /// </summary>
    public abstract class CommandEventArgs : System.EventArgs
    {
        public abstract string Arguments { get; }
        public abstract string Command { get; }
        
        public abstract Stopwatch Stopwatch { get; }
        public abstract VolteContext Context { get; }
    }
}
