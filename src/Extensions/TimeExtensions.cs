using System;

namespace Volte.Extensions
{
    public static class TimeExtensions
    {
        public static string FormatFullTime(this DateTimeOffset offset)
        {
            var hour = offset.Hour > 9 ? $"{offset.Hour}" : $"0{offset.Hour}";
            var min = offset.Minute > 9 ? $"{offset.Minute}" : $"0{offset.Minute}";
            var sec = offset.Second > 9 ? $"{offset.Second}" : $"0{offset.Second}";
            return $"{hour}:{min}:{sec}";
        }

        public static string FormatPartialTime(this DateTimeOffset offset)
        {
            var min = offset.Minute > 9 ? $"{offset.Minute}" : $"0{offset.Minute}";
            var sec = offset.Second > 9 ? $"{offset.Second}" : $"0{offset.Second}";
            return $"{min}:{sec}";
        }

        public static string FormatDate(this DateTimeOffset offset)
        {
            var day = offset.Day > 9 ? $"{offset.Day}" : $"0{offset.Day}";
            var month = offset.Month > 9 ? $"{offset.Month}" : $"0{offset.Month}";
            var year = $"{offset.Year}".Replace("20", string.Empty);
            return $"{day}/{month}/{year}";
        }
    }
}
