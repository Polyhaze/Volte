using Discord;
using Discord.Commands;

namespace Volte.Data.Objects
{
    public enum LogSource
    {
        Module,
        Service,
        Discord,
        Rest,
        Gateway,
        Volte,
        Unknown
    }
}