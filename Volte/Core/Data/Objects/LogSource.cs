using Discord;
using Discord.Commands;

namespace Volte.Core.Data.Objects
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