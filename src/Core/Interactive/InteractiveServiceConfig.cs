using System;

namespace Volte.Interactive
{
    public class InteractiveServiceConfig
    {
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(15);
    }
}
