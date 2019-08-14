namespace Volte.Core.Models
{
    public enum ModActionType : uint
    {
        Purge = 1,
        Warn = 2,
        ClearWarns = 3,
        Delete = 4,
        Kick = 5,
        Softban = 6,
        IdBan = 7,
        Ban = 8
    }
}