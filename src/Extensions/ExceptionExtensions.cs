using System;
using System.Threading.Tasks;
using Discord;
using Volte.Data.Objects;
using Volte.Discord;
using Volte.Services;

namespace Volte.Extensions
{
    public static class ExceptionExtensions
    {

        public static Task PrintStackTrace(this Exception e)
        {
            var logger = VolteBot.GetRequiredService<LoggingService>();
            return logger.Log(LogSeverity.Error, LogSource.Volte, string.Empty, e);
        }

    }
}
