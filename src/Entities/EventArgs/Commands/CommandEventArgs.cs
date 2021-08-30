using System;
using System.Diagnostics;
using Volte.Commands;

namespace Volte.Entities
{
    /// <summary>
    ///     The base class for all Command-related Volte EventArgs.
    /// </summary>
    public abstract class CommandEventArgs : EventArgs
    {
        public string Arguments { get; set; }
        public string Command { get; set; }
        
        public Stopwatch Stopwatch { get; set; }
        public VolteContext Context { get; set; }
    }
}
