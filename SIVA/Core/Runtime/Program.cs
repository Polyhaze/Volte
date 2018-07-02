using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SIVA.Core.Discord;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Runtime
{
    internal class Program
    {
        
        private static void Main()
        {
            Console.Title = "SIVA";
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Red;
            DiscordLogin.LoginAsync().GetAwaiter().GetResult();
        }

    }
}
