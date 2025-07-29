using System.Globalization;
using System.IO;

namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class BotOwnerModule
{
    [Command("DevInfo", "Di")]
    [Description("Shows information about the bot and about the system it's hosted on.")]
    public Task<ActionResult> DevInfoAsync()
        => Ok(Format.Code(new StringBuilder()
            .AppendLine("== Core ==")
            .AppendLine($"[{Context.Client.Guilds.Count}] Guilds")
            .AppendLine($"[{Context.Client.Guilds.Sum(static x => x.Channels.Count)}] Text/Voice Channels")
            .AppendLine("== Commands ==")
            .AppendLine($"[{CommandService.GetAllModules().Count}] Modules")
            .AppendLine($"[{CommandService.GetAllCommands().Count}] Commands")
            .AppendLine($"[{CommandService.GetTotalTypeParsers()}] TypeParsers")
            .AppendLine("== Environment ==")
            .AppendLine($"Current Directory: [{Directory.GetCurrentDirectory()}]")
            .AppendLine($"               OS: [{Environment.OSVersion}]")
            .AppendLine($"  Processor Count: [{Environment.ProcessorCount}]")
            .AppendLine($"     Is 64-bit OS: [{Environment.Is64BitOperatingSystem}]")
            .AppendLine($"Is 64-bit Process: [{Environment.Is64BitProcess}]")
            .AppendLine($"Current Thread ID: [{Environment.CurrentManagedThreadId}]")
            .AppendLine($"Host Machine Name: [{Environment.MachineName}]")
            .AppendLine($"     .NET Version: [{Environment.Version}]")
            .AppendLine($"       UI Culture: [{CultureInfo.CurrentUICulture.EnglishName}]")
            .AppendLine($" System Directory: [{Environment.SystemDirectory}]")
            .ToString(), "ini"));
}