using System;
using Discord;
using SIVA.Core.Discord;

namespace SIVA.Core.Runtime
{
    internal class Program
    {
        private static void Main()
        {
            Console.Title = "SIVA";
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Currently running SIVA Version {Version.GetFullVersion()}.");
            DiscordLogin.LoginAsync().GetAwaiter().GetResult();
        }
    }
}
