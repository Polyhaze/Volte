using System;
using System.IO;
using Discord;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Runtime {
    internal class Program {
        private static void Main() {
            Console.Title = "SIVA";
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Red;
            if (new Program().InitSIVA()) {
                new Discord.SIVA();
            } 
        }

        private bool InitSIVA() {
            if (!Directory.Exists("data")) {
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