using Microsoft.Extensions.Logging;
using Volte.Core.Entities;

namespace Gommon
{
    public static partial class Extensions
    {
        public static LogSource GetSource(this EventId id)
        {
            return LogSourceAttribute.EventIdMappings.TryGetValue(id.Id, out var source) ? source : LogSource.Unknown;
        }
    }
}