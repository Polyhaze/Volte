namespace Volte.Data.Models
{
    /// <summary>
    ///     Model that represents enabled/disabled features as defined in your config.
    /// </summary>
    public sealed class EnabledFeatures
    {
        internal EnabledFeatures() { } //restrict non-Volte assembly instantiation

        public bool Antilink { get; } = true;
        public bool Blacklist { get; } = true;
        public bool ModLog { get; } = true;
        public bool Welcome { get; } = true;
        public bool Autorole { get; } = true;
        public bool PingChecks { get; } = true;
    }
}