using System.Diagnostics;
using Qmmands;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using Volte.Commands;

namespace Volte.Core.Models.EventArgs
{
    /// <summary>
    ///     The base class for all Command-related Volte EventArgs.
    /// </summary>
    public abstract class CommandEventArgs : System.EventArgs
    {
        public abstract string FullMessageContent { get; }
        public abstract IResult Result { get; }
        public abstract Stopwatch Stopwatch { get; }
        public abstract VolteContext Context { get; }
    }
}
