using System;
using System.Threading.Tasks;
using Discord;
using Volte.Data.Models;
using Volte.Services;

namespace Volte.Extensions
{
    public static class ExceptionExtensions
    {
        public static Task PrintStackTraceAsync(this Exception e) 
            => LoggingService.Instance.LogAsync(LogSeverity.Error, LogSource.Volte, string.Empty, e);

    }
}
