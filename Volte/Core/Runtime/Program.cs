using System;
using System.IO;
using Volte.Core.Discord;
using Volte.Core.Data;

namespace Volte.Core.Runtime {
    internal static class Program {
        private static void Main(string[] args) {
            Console.Title = "Volte"; 
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Red;
            
            if (InitVolte()) {
                // ReSharper disable once ObjectCreationAsStatement
                new VolteBot();
            }
        }

        private static bool InitVolte() {
            
            if (!Directory.Exists("data")) {
                new Logger().Fatal("The \"data\" directory has been created. Please fill in the config.");
                Directory.CreateDirectory("data");
                return false;
            }

            if (string.IsNullOrEmpty(Config.GetToken())) {
                new Logger().Fatal(
                    "You haven't setup Volte's config. " +
                    "Please do so before starting the bot. " +
                    "A file under the \"data\" directory has been created for you.");
                return false;
            }

            return true;
        }
    }
}