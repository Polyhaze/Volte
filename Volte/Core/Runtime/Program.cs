using System;
using System.IO;
using System.Threading;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Data;
using Volte.Core.Services;

namespace Volte.Core.Runtime {
    internal static class Program {
        private static void Main() {
            Console.Title = "Volte";
            Console.CursorVisible = false;
            InitVolte();
            // ReSharper disable once ObjectCreationAsStatement
            VolteBot.Start();
        }

        private static void InitVolte() {
            if (!Directory.Exists("data")) {
                VolteBot.ServiceProvider.GetRequiredService<LoggingService>()
                    .Log(LogSeverity.Critical, "Volte",
                        "The \"data\" directory didn't exist, so I created it for you.");
                Directory.CreateDirectory("data");
            }
        }
    }
}