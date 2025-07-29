namespace Volte.Systems.Interactive;

public class InteractiveServiceConfig
{
    public TimeSpan DefaultTimeout { get; }

    public InteractiveServiceConfig(TimeSpan? defaultTimeout = null)
    {
        DefaultTimeout = defaultTimeout ?? 15.Seconds();
    }
}