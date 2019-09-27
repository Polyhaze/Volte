using Volte.Commands;

namespace Volte.Core.Models.EventArgs
{
    /// <summary>
    ///     The base class for all Command-related Volte EventArgs.
    /// </summary>
    public abstract class CommandEventArgs : System.EventArgs
    {
        public abstract VolteContext Context { get; }
    }
}
