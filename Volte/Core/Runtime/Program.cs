using System;
using System.IO;
using System.Threading;
using Volte.Core.Discord;
using Volte.Core.Data;

namespace Volte.Core.Runtime {
    internal static class Program {
        private static void Main() {
            Console.Title = "Volte"; 
            Console.CursorVisible = false;
            
            if (InitVolte()) {
                // ReSharper disable once ObjectCreationAsStatement
                new VolteBot();
            }
        }

        private static bool InitVolte() {
            if (!Directory.Exists("data")) {
                new Logger().Fatal("The \"data\" directory has been created. Please fill in the config.");
                Directory.CreateDirectory("data");
            }
            return true;
        }
    }
}