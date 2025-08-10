namespace Volte.Systems.Database.EntitiesV2;

public class StarboardSettings
{
    public Snowflake Channel { get; set; }
    
    public bool Enabled { get; set; }
    
    public int StarsRequiredToPost { get; set; } = 1;
    
    public bool DeleteInvalidStars { get; set; }
}