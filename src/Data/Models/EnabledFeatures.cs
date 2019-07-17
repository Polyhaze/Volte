namespace Volte.Data.Models
{
    /// <summary>
    ///     Model that represents enabled/disabled features as defined in your config.
    /// </summary>
    public sealed class EnabledFeatures
    {
        internal EnabledFeatures() { } //restrict non-Volte assembly instantiation

        public bool Antilink { get; set; } = true;
        public bool Blacklist { get; set; } = true;
        public bool ModLog { get; set; } = true;
        public bool Welcome { get; set; } = true;
        public bool Autorole { get; set; } = true;
        public bool PingChecks { get; set; } = true;

    }
}
