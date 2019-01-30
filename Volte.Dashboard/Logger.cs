using System;
using System.Data;
using Microsoft.Extensions.Logging;

namespace Volte.Dashboard {
    public class Logger : ILogger {
        public static Logger GetLogger() {
            return new Logger();
        }

        public void Info(string message) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[INFO]: " + message);
        }

        public void Warn(string message) {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[WARN]: " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Error(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR]: " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Fatal(string message) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("[FATAL]: " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Debug(string message) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[DEBUG]: " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter) {
            if (!IsEnabled(logLevel)) return;
            switch (logLevel) {
                case LogLevel.Debug: {
                    Debug(formatter(state, exception));
                    break;
                }
                case LogLevel.Error: {
                    Error(formatter(state, exception));
                    break;
                }
                case LogLevel.Warning: {
                    Warn(formatter(state, exception));
                    break;
                }
                case LogLevel.Information: {
                    Info(formatter(state, exception));
                    break;
                }
            }
        }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state) {
            throw new NotImplementedException();
        }
    }
}