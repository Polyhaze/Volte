using System;
using Humanizer;

namespace Volte.Interactive
{
    public class InteractiveServiceConfig
    {
        public TimeSpan DefaultTimeout { get; set; } = 15.Seconds();
    }
}
