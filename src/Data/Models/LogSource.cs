using Discord;
using Discord.Commands;

namespace Volte.Data.Models
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