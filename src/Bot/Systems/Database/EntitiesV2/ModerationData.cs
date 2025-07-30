namespace Volte.Systems.Database.EntitiesV2;

public class ModerationData
{
    public ulong CurrentModActionCase { get; set; }
    
    public HashSet<WarnV2> Warns { get; set; }
}