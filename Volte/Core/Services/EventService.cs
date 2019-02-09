using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Commands;
using Volte.Core.Discord;
using Volte.Core.Data;
using Volte.Core.Data.Objects;
using Volte.Core.Extensions;
using Volte.Core.Runtime;
using Volte.Helpers;

#pragma warning disable 1998
namespace Volte.Core.Services {
    internal class EventService {
        private readonly LoggingService _logger = VolteBot.ServiceProvider.GetRequiredService<LoggingService>();

        public async Task OnReady() {
            var dbl = VolteBot.Client.GetGuild(264445053596991498);
            if (dbl is null || Config.GetOwner() == 168548441939509248) return;
            await dbl.GetTextChannel(265156286406983680).SendMessageAsync(
                $"<@168548441939509248>: I am a Volte not owned by you. Please do not post Volte to a bot list again, <@{Config.GetOwner()}>.");
            await dbl.LeaveAsync();
        }

        public async Task Guilds(SocketGuild guild) {
            /*if (Config.GetBlacklistedOwners().Contains(guild.OwnerId)) {
                await guild.LeaveAsync();
                return;
            }*/

            var embed = new EmbedBuilder()
                .WithTitle("Hey there!")
                .WithAuthor(guild.Owner)
                .WithColor(Config.GetSuccessColor())
                .WithDescription("Thanks for inviting me! Here's some basic instructions on how to set me up.")
                .AddField("Set your admin role", "$adminrole {roleName}", true)
                .AddField("Set your moderator role", "$modrole {roleName}", true)
                .AddField("Permissions",
                    "It is recommended to give me admin permission, to avoid any permission errors that may happen." +
                    "\nYou *can* get away with just send messages, ban members, kick members, and the like if you don't want to give me admin.")
                .AddField("Support Server", "[Join my support Discord here](https://discord.gg/H8bcFr2)");
            
            try {
                await embed.SendTo(guild.Owner);
            }
            catch (HttpException e) when (e.DiscordCode.Equals(50007)) {
                var c = guild.TextChannels?.First();
                if (c != null) {
                    await embed.SendTo(c);
                }
            }
        }


        public async Task OnCommand(Optional<CommandInfo> cinfo, ICommandContext context, IResult res) {
            var ctx = (VolteContext)context;
            if (!cinfo.IsSpecified) return;
            var config = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild);
            var commandName = ctx.Message.Content.Split(" ")[0];
            var args = ctx.Message.Content.Replace($"{commandName}", "");
            if (string.IsNullOrEmpty(args)) {
                args = "None";
            }
            
            var argPos = 0;
            var embed = new EmbedBuilder();
            if (!res.IsSuccess && res.ErrorReason != "Unknown command." && res.ErrorReason != "Insufficient permission.") {
                string reason;
                switch (res.ErrorReason) {
                    case "The server responded with error 403: Forbidden":
                        reason =
                            "I'm not allowed to do that. " +
                            "Either I don't have permission, " +
                            "or the requested user is higher " +
                            "than me in the role hierarchy.";
                        break;
                    case "Failed to parse Boolean.":
                        reason = "You can only input `true` or `false` for this command.";
                        break;
                    default:
                        reason = res.ErrorReason;
                        break;
                }
                
                var aliases = cinfo.Value.Aliases.Aggregate("(", (current, alias) => current + alias + "|");

                aliases += ")";
                aliases = aliases.Replace("|)", ")");
                
                if (ctx.Message.HasMentionPrefix(VolteBot.Client.CurrentUser, ref argPos)) {
                    embed.AddField("Error in Command:", cinfo.Value.Name);
                    embed.AddField("Error Reason:", reason);
                    embed.AddField("Correct Usage", cinfo.Value.Remarks
                        .Replace("Usage: ", string.Empty)
                        .Replace("|prefix|", config.CommandPrefix)
                        .Replace($"{cinfo.Value.Name.ToLower()}", aliases));
                    embed.WithAuthor(ctx.User);
                    embed.WithColor(Config.GetErrorColor());
                    await Utils.Send(ctx.Channel, embed.Build());
                }
                else {
                    embed.AddField("Error in Command:", cinfo.Value.Name);
                    embed.AddField("Error Reason:", reason);
                    embed.AddField("Correct Usage", cinfo.Value.Remarks
                        .Replace("Usage: ", string.Empty)
                        .Replace("|prefix|", config.CommandPrefix)
                        .Replace($"{cinfo.Value.Name.ToLower()}", aliases));
                    embed.WithAuthor(ctx.User);
                    embed.WithColor(Config.GetErrorColor());
                    await Utils.Send(ctx.Channel, embed.Build());
                }
            }
            

            if (Config.GetLogAllCommands()) {
                if (res.IsSuccess) {
                    _logger.Log(LogSeverity.Info, LogSource.Module,
                        $"|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator}");
                    _logger.Log(LogSeverity.Info, LogSource.Module,
                        $"|     -Command Issued: {cinfo.Value.Name}");
                    _logger.Log(LogSeverity.Info, LogSource.Module,
                        $"|        -Args Passed: {args.Trim()}");
                    _logger.Log(LogSeverity.Info, LogSource.Module,
                        $"|           -In Guild: {ctx.Guild.Name}");
                    _logger.Log(LogSeverity.Info, LogSource.Module,
                        $"|         -In Channel: #{ctx.Channel.Name}");
                    _logger.Log(LogSeverity.Info, LogSource.Module,
                        $"|        -Time Issued: {DateTime.Now}");
                    _logger.Log(LogSeverity.Info, LogSource.Module,
                        $"|           -Executed: {res.IsSuccess} ");
                    _logger.Log(LogSeverity.Info, LogSource.Module,
                        "-------------------------------------------------");
                    



                }
                else {
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|     -Command Issued: {cinfo.Value.Name}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|        -Args Passed: {args.Trim()}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|           -In Guild: {ctx.Guild.Name}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|         -In Channel: #{ctx.Channel.Name}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|        -Time Issued: {DateTime.Now}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|           -Executed: {res.IsSuccess} | Reason: {res.ErrorReason}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        "-------------------------------------------------");
                }
            }
        }
    }
}