using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Data;
using Volte.Core.Runtime;
using Volte.Helpers;

#pragma warning disable 1998
namespace Volte.Core.Services {
    internal class EventService {
        private readonly Logger _logger = Logger.GetLogger();

        public async Task OnReady() {
            var dbl = VolteBot.Client.GetGuild(264445053596991498);
            if (dbl is null || Config.GetOwner() == 168548441939509248) return;
            await dbl.GetTextChannel(265156286406983680).SendMessageAsync(
                $"<@168548441939509248>: I am a Volte not owned by you. Please do not post Volte to a bot list again, <@{Config.GetOwner()}>.");
            await dbl.LeaveAsync();
        }

        public async Task Guilds(SocketGuild guild) {
            if (Config.GetBlacklistedOwners().Contains(guild.OwnerId)) {
                await guild.LeaveAsync();
            }
        }


        public async Task OnCommand(Optional<CommandInfo> cinfo, ICommandContext ctx, IResult res) {
            if (!cinfo.IsSpecified) return;
            var config = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild);
            var argPos = 0;
            var embed = new EmbedBuilder();
            if (!res.IsSuccess && res.ErrorReason != "Unknown command.") {
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
                
                if (ctx.Message.HasMentionPrefix(VolteBot.Client.CurrentUser, ref argPos)) {
                    embed.AddField("Error in Command:", cinfo.Value.Name);
                    embed.AddField("Error Reason:", reason);
                    embed.AddField("Correct Usage", cinfo.Value.Remarks.Replace("Usage: ", string.Empty));
                    embed.WithAuthor(ctx.User);
                    embed.WithColor(Config.GetErrorColor());
                    await Utils.Send(ctx.Channel, embed.Build());
                }
                else {
                    embed.AddField("Error in Command:", cinfo.Value.Name);
                    embed.AddField("Error Reason:", reason);
                    embed.AddField("Correct Usage", cinfo.Value.Remarks.Replace("Usage: ", string.Empty));
                    embed.WithAuthor(ctx.User);
                    embed.WithColor(Config.GetErrorColor());
                    await Utils.Send(ctx.Channel, embed.Build());
                }
            }
            

            if (Config.GetLogAllCommands()) {
                if (res.IsSuccess) {
                    _logger.Info($"--|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator}");
                    _logger.Info($"--|     -Command Issued: {cinfo.Value.Name}");
                    _logger.Info("--|        -Args Passed: " +
                                 ctx.Message.Content.Replace(
                                     $"{config.CommandPrefix}{cinfo.Value.Name} ",
                                     string.Empty, StringComparison.CurrentCultureIgnoreCase
                                 ));
                    _logger.Info($"--|           -In Guild: {ctx.Guild.Name}");
                    _logger.Info($"--|         -In Channel: #{ctx.Channel.Name}");
                    _logger.Info($"--|        -Time Issued: {DateTime.Now}");
                    _logger.Info($"--|           -Executed: {res.IsSuccess} ");
                    _logger.Info("-------------------------------------------------");
                }
                else {
                    _logger.Error($"--|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator}");
                    _logger.Error($"--|     -Command Issued: {cinfo.Value.Name}");
                    _logger.Error("--|        -Args Passed: " +
                                  ctx.Message.Content.Replace(
                                      $"{config.CommandPrefix}{cinfo.Value.Name} ",
                                      string.Empty,
                                      StringComparison.CurrentCultureIgnoreCase
                                  ));
                    _logger.Error($"--|           -In Guild: {ctx.Guild.Name}");
                    _logger.Error($"--|         -In Channel: #{ctx.Channel.Name}");
                    _logger.Error($"--|        -Time Issued: {DateTime.Now}");
                    _logger.Error($"--|           -Executed: {res.IsSuccess} | Reason: {res.ErrorReason}");
                    _logger.Error("-------------------------------------------------");
                }
            }
        }
    }
}