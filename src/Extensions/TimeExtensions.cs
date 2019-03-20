using System;

namespace Volte.Extensions
{
    public static class TimeExtensions
    {
        public static string FormatFullTime(this DateTimeOffset offset)
        {
            return offset.ToString("hh:mm:ss tt");
        }

        public static string FormatPartialTime(this DateTimeOffset offset)
        {
            return offset.ToString("mm:ss tt");
        }

        public static string FormatDate(this DateTimeOffset offset)
        {
            return offset.ToString("MM/dd/yyyy");
        }
    }
}