using System;
using Discord;

namespace SIVA.Core.Runtime {
    internal class Program {
        private static void Main() {
            Console.Title = "SIVA";
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Red;
            new Discord.SIVA();
        }
    }
}