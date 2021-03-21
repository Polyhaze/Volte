using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("DevInfo", "Di")]
        [Description("Shows information about the bot and about the system it's hosted on.")]
        public Task<ActionResult> DevInfoAsync() 
            => Ok(Format.Code(new StringBuilder()
                    .AppendLine("== Core ==")
                    .AppendLine($"[{Context.Client.Guilds.Count}] Guilds")
                    .AppendLine($"[{Context.Client.Guilds.Sum(x => x.Channels.Count)}] Text/Voice Channels")
                    .AppendLine("== Commands ==")
                    .AppendLine($"[{CommandService.GetAllModules().Count}] Modules")
                    .AppendLine($"[{CommandService.GetAllCommands().Count}] Commands")
                    .AppendLine($"[{CommandService.GetTotalTypeParsers()}] TypeParsers")
                    .AppendLine("== Environment ==")
                    .AppendLine($"OS: [{Environment.OSVersion}]")
                    .AppendLine($"Processor Count: [{Environment.ProcessorCount}]")
                    .AppendLine($"Is 64-bit OS: [{Environment.Is64BitOperatingSystem}]")
                    .AppendLine($"Is 64-bit Process: [{Environment.Is64BitProcess}]")
                    .AppendLine($"Current Managed Thread ID: [{Environment.CurrentManagedThreadId}]")
                    .AppendLine($"Machine Name: [{Environment.MachineName}]")
                    .AppendLine($".NET Core Version: [{Environment.Version}]")
                    .AppendLine($"UICulture: [{CultureInfo.CurrentUICulture.EnglishName}]")
                    .AppendLine($"System Directory: [{Environment.SystemDirectory}]")
                    .ToString(), "ini"));

    }
}
