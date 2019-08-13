using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Humanizer;

namespace Volte.Helpers
{
    public static class CalculationHelper
    {

        public static string GetUptime() 
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize(3);

    }
}
