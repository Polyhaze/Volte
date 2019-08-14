namespace Volte.Core.Models
{
    public enum LogSource : uint
    {
        Module = 1,
        Service = 2,
        Discord = 3,
        Rest = 4,
        Gateway = 5,
        Volte = 6,
        Unknown = uint.MaxValue
    }
}