namespace Volte.Systems.Database.EntitiesV2;

public class WarnV2
{
    public Snowflake Target { get;  set; }
    public string Reason { get; set; }
    public Snowflake Issuer { get; set; }
    public DateTimeOffset Date { get; set; }
}