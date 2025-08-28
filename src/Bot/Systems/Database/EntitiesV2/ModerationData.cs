namespace Volte.Systems.Database.EntitiesV2;

public class ModerationData
{
    public ulong CurrentModActionCase { get; set; }

    public HashSet<WarnV2> Warns { get; set; }

    public bool AddWarn(WarnInitializer warnInitializer)
    {
        var warn = new WarnV2();
        warnInitializer(warn);

        return AddWarn(warn);
    }

    public bool AddWarn(WarnV2 warn) => Warns.Add(warn);
}