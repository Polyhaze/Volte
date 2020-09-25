using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Core.Helpers;
using Volte.Services;

// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Commands.Modules
{
    [RequireBotOwner]
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("Shutdown")]
        [Description("Forces the bot to shutdown.")]
        [Remarks("shutdown")]
        public Task<ActionResult> ShutdownAsync()
            => Ok($"Goodbye! {EmojiHelper.Wave}", _ =>
            {
                Cts.Cancel();
                return Task.CompletedTask;
            });

        [Command("SetStream")]
        [Description("Sets the bot's stream via Twitch username and Stream name, respectively.")]
        [Remarks("setstream {String} {String}")]
        public Task<ActionResult> SetStreamAsync([RequiredArgument] string stream,
            [Remainder, RequiredArgument] string game)
        {
            return Ok(
                $"Set the bot's game to **{game}**, and the Twitch URL to **[{stream}](https://twitch.tv/{stream})**.",
                _ => Context.Client.UpdateStatusAsync(new DiscordActivity
                {
                    Name = game,
                    ActivityType = ActivityType.Streaming,
                    StreamUrl = $"https://twitch.tv/{stream}"
                }));
        }

        [Command("SetStatus")]
        [Description("Sets the bot's status.")]
        [Remarks("setstatus {dnd|idle|invisible|online}")]
        public Task<ActionResult> SetStatusAsync([Remainder, RequiredArgument] UserStatus status)
        {
            return Ok($"Set my status to `{status}`!", 
                _ => Context.Client.UpdateStatusAsync(Cache.GetBotPresence(Context.Client).Activity, status));
        }

        [Command("SetAvatar")]
        [Description("Sets the bot's avatar to the image at the given URL.")]
        [Remarks("setavatar {String}")]
        public async Task<ActionResult> SetAvatarAsync([RequiredArgument] string url)
        {
            if (url.IsNullOrWhitespace() || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return BadRequest("That URL is malformed or empty.");

            using var sr = await Http.Client.GetAsync(url);

            if (!sr.IsImage())
                return BadRequest(
                    "Provided URL does not lead to an image. Note that I cannot follow redirects; " +
                    "so provide *direct* image URLs please! An example of this is an i.imgur.com link.");

            await using var img = (await sr.Content.ReadAsByteArrayAsync()).ToStream();
            img.Position = 0;
            await Context.Client.UpdateCurrentUserAsync(avatar: img);
            return Ok("Done!");
        }

        [Command("SetName")]
        [Description("Sets the bot's username.")]
        [Remarks("setname {String}")]
        public Task<ActionResult> SetNameAsync([Remainder, RequiredArgument] string name)
            => Ok($"Set my username to **{name}**.", _ => Context.Client.UpdateCurrentUserAsync(name));

        [Command("SetGame")]
        [Description("Sets the bot's game (presence).")]
        [Remarks("setgame {String}")]
        public Task<ActionResult> SetGameAsync([Remainder, RequiredArgument] string game) 
            => Ok($"Set my game to {game}!", _ => Context.Client.UpdateStatusAsync(new DiscordActivity(game, ActivityType.Playing)));

        [Command("Reload", "Rl")]
        [Description(
            "Reloads the bot's configuration file. NOTE: This will throw an exception if the config file is invalid JSON!")]
        [Remarks("reload")]
        public async Task<ActionResult> ReloadAsync()
        {
            var (success, error) = Config.Reload();
            return success
                ? Ok("Config reloaded!")
                : BadRequest(
                    $"Something bad happened. Further exception information can be found [here]({await Http.PostToGreemPasteAsync(error.StackTrace ?? error.Message)}).");
        }

        [Command("Eval", "Evaluate")]
        [Description("Evaluates C# code.")]
        [Remarks("eval {String}")]
        public Task<ActionResult> EvalAsync([Remainder, RequiredArgument] string code)
            => None(async () => await Eval.EvaluateAsync(this, code), false);

        [Hidden, Command("Inspect", "Insp")]
        [Description("Inspects a .NET object.")]
        [Remarks("inspect {String}")]
        public Task<ActionResult> InspectAsync([Remainder, RequiredArgument] string obj)
            => EvalAsync($"Inspect({obj})");

        [Hidden, Command("Inheritance", "Inh")]
        [Description("Shows the inheritance tree of a .NET type.")]
        [Remarks("inheritance {String}")]
        public Task<ActionResult> InheritanceAsync([RequiredArgument] string type)
            => EvalAsync($"Inheritance<{type}>()");

        [Command("ForceLeave")]
        [Description("Forcefully leaves the guild with the given name.")]
        [Remarks("forceleave {Guild}")]
        public Task<ActionResult> ForceLeaveAsync([Remainder, RequiredArgument] DiscordGuild guild) 
            => Ok($"Successfully left **{guild.Name}**.", _ => guild.LeaveAsync());

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
                .AppendLine($"Used Memory: [{Process.GetCurrentProcess().GetMemoryUsage()}]")
                .AppendLine($"Processor Count: [{Environment.ProcessorCount}]")
                .AppendLine($"Is 64-bit OS: [{Environment.Is64BitOperatingSystem}]")
                .AppendLine($"Is 64-bit Process: [{Environment.Is64BitProcess}]")
                .AppendLine($"Current Managed Thread ID: [{Environment.CurrentManagedThreadId}]")
                .AppendLine($"Machine Name: [{Environment.MachineName}]")
                .AppendLine($".NET Core Version: [{Environment.Version}]")
                .AppendLine($"UICulture: [{CultureInfo.InstalledUICulture.EnglishName}]")
                .AppendLine($"System Directory: [{Environment.SystemDirectory}]")
                .ToString(), "ini"));

        [Command("CreateConfig")]
        [Description("Create a config for the guild with the given ID, if one somehow doesn't exist.")]
        [Remarks("createconfig [Guild]")]
        [Hidden]
        public Task<ActionResult> CreateConfigAsync([Remainder, OptionalArgument] DiscordGuild guild = null)
        {
            guild ??= Context.Guild;
            return Ok($"Created a config for **{guild.Name}** if it didn't exist. It probably did so that was most likely pointless, but I digress.", m =>
            {
                _ = Db.GetData(guild.Id);
                return Task.CompletedTask;
            });
        }
    }
}