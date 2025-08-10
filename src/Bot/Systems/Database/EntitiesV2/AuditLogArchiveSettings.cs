namespace Volte.Systems.Database.EntitiesV2;

public class AuditLogArchiveSettings
{
    public Snowflake Channel { get; set; }
    public Dictionary<ActionType, bool> EnabledLogs { get; set; }

    public void Toggle(ActionType type)
    {
        if (EnabledLogs.TryGetValue(type, out bool value))
            EnabledLogs[type] = !value;
        else
            EnabledLogs.Add(type, true);
    }
    
    public bool IsEnabled(ActionType type) => EnabledLogs.GetValueOrDefault(type, true);

    public static AuditLogArchiveSettings Empty => new()
    {
        Channel = Snowflake.Zero,
        EnabledLogs = Enum.GetValues<ActionType>().ToDictionary(x => x, _ => true)
    };
}