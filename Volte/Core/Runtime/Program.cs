using System;
using System.IO;
using Volte.Core.Discord;
using Volte.Core.Files.Readers;

namespace Volte.Core.Runtime {
    internal static class Program {
        private static void Main() {
            Console.Title = "SIVA"; 
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Red;
            
            if (InitSIVA()) {
                new VolteBot();
            } 
        }

        private static bool InitSIVA() {
            
            if (!Directory.Exists("data")) {
                new Log().Fatal("The \"data\" directory has been created. Please fill in the config.");
                Directory.CreateDirectory("data");
                return false;
            }

            if (string.IsNullOrEmpty(Config.GetToken())) {
                new Log().Fatal(
                    "You haven't setup SIVA's config. " +
                    "Please do so before starting the bot. " +
                    "A file under the \"data\" directory has been created for you.");
                return false;
            }

            return true;
        }
    }
}