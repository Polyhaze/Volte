using System;

namespace SIVA.Core.Runtime {
    public class Log {
        public static Log GetLogger() {
            return new Log();
        }

        public void Info(string message) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[INFO]: " + message);
        }

        public void Warn(string message) {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[WARN]: " + message);
        }

        public void Fatal(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[FATAL]: " + message);
        }

        public void PrintVersion() {
            Console.WriteLine($"Currently running SIVA V{Version.GetFullVersion()}!");
        }
    }
}