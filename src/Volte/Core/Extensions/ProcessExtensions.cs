using System;
using System.Diagnostics;
using Humanizer;

namespace Gommon
{
    public static partial class Extensions
    {
        public static string GetUptime(this Process process)
            => (DateTime.Now - process.StartTime).Humanize(3);

    }
}
