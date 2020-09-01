using System;
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using RestSharp.Extensions;

namespace Volte.Core.Entities 
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LogSourceAttribute : Attribute
    {
        public static IReadOnlyDictionary<LogSource, LogSourceAttribute> LogSources { get; }
        public static IReadOnlyDictionary<int, LogSource> EventIdMappings { get; }

        static LogSourceAttribute()
        {
            var logSources = new Dictionary<LogSource, LogSourceAttribute>();
            foreach (LogSource logSource in Enum.GetValues(typeof(LogSource)))
            {
                var name = logSource.ToString();
                var attr = typeof(LogSource).GetField(name)!
                    .GetAttribute<LogSourceAttribute>();
                attr.Name ??= name;
                logSources[logSource] = attr;
            }
            LogSources = logSources;
            
            var eventIdMappings = new Dictionary<int, LogSource>();
            foreach (var (logSource, attr) in LogSources)
            {
                if (attr.MapsToEventId != 0)
                {
                    eventIdMappings[attr.MapsToEventId] = logSource;
                }
            }
            EventIdMappings = eventIdMappings;
        }
        
        public Color Color { get; }

        [CanBeNull] private string _name;
        [CanBeNull]
        public string Name
        {
            get => _name;
            set => _name = value?.ToUpperInvariant() ?? "UNKNOWN";
        }

        public int MapsToEventId { get; set; }

        public LogSourceAttribute(KnownColor color)
        {
            Color = Color.FromKnownColor(color);
        }
    }
}