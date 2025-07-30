namespace Volte.Systems.Database.EntitiesV2;

public class WarnV2
{
    public ulong Target { get;  set; }
    public string Reason { get; set; }
    public ulong Issuer { get; set; }
    public DateTimeOffset Date { get; set; }
}