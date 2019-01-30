using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Volte.Dashboard {
    public class LoggerProvider : ILoggerProvider {
        
        private readonly ConcurrentDictionary<string, Logger> _loggers = new ConcurrentDictionary<string, Logger>();
        
        public void Dispose() {
            _loggers.Clear();
        }

        public ILogger CreateLogger(string categoryName) {
            return _loggers.GetOrAdd(categoryName, ignored => new Logger());
        }
    }
}