using System;
using System.Threading.Tasks;
using Discord;
using Volte.Data.Models;
using Volte.Core;
using Volte.Services;

namespace Volte.Extensions
{
    public static class ExceptionExtensions
    {

        public static Task PrintStackTrace(this Exception e)
        {
            var logger = VolteBot.GetRequiredService<LoggingService>();
            return logger.LogAsync(LogSeverity.Error, LogSource.Volte, string.Empty, e);
        }

    }
}
