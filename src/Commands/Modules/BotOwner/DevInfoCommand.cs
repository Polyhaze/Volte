using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("DevInfo", "Di")]
        [Description("Shows information about the bot and about the system it's hosted on.")]
        [Remarks("devinfo")]
        public Task<ActionResult> DevInfoAsync() 
            => Ok(Formatter.BlockCode(new StringBuilder()
                    .AppendLine("== Core ==")
                    .AppendLine($"[{Context.Client.GetGuildCount()}] Guilds")
                    .AppendLine($"[{Context.Client.GetChannelCount()}] Text/Voice Channels")
                    .AppendLine("== Commands ==")
                    .AppendLine($"[{CommandService.GetAllModules().Count}] Modules")
                    .AppendLine($"[{CommandService.GetAllCommands().Count}] Commands")
                    .AppendLine($"[{CommandService.GetTotalTypeParsers()}] TypeParsers")
                    .AppendLine("== Environment ==")
                    .AppendLine($"OS: [{Environment.OSVersion}]")
                    .AppendLine($"Current Uptime: [{Process.GetCurrentProcess().CalculateUptime()}]")
                    .AppendLine($"Used Memory: [{Process.GetCurrentProcess().GetMemoryUsage(MemoryType.Megabytes)}]")
                    .AppendLine($"Processor Count: [{Environment.ProcessorCount}]")
                    .AppendLine($"Is 64-bit OS: [{Environment.Is64BitOperatingSystem}]")
                    .AppendLine($"Is 64-bit Process: [{Environment.Is64BitProcess}]")
                    .AppendLine($"Current Managed Thread ID: [{Environment.CurrentManagedThreadId}]")
                    .AppendLine($"Machine Name: [{Environment.MachineName}]")
                    .AppendLine($".NET Core Version: [{Environment.Version}]")
                    .AppendLine($"UICulture: [{CultureInfo.InstalledUICulture.EnglishName}]")
                    .AppendLine($"System Directory: [{Environment.SystemDirectory}]")
                    .ToString(), "ini"));

    }
}
