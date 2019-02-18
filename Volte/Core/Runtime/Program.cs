using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Data.Objects;
using Volte.Core.Discord;
using Volte.Core.Services;

namespace Volte.Core.Runtime
{
    internal static class Program
    {
        private static async Task Main()
        {
            Console.Title = "Volte";
            Console.CursorVisible = false;
            await InitVolte();
            // ReSharper disable once ObjectCreationAsStatement
            await VolteBot.Start();
        }

        private static async Task InitVolte()
        {
            if (!Directory.Exists("data"))
            {
                await VolteBot.ServiceProvider.GetRequiredService<LoggingService>()
                    .Log(LogSeverity.Critical, LogSource.Volte,
                        "The \"data\" directory didn't exist, so I created it for you.");
                Directory.CreateDirectory("data");
            }
        }
    }
}