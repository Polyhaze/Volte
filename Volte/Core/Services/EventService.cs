using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Files.Readers;
using Volte.Core.Modules;
using Volte.Core.Runtime;

#pragma warning disable 1998
namespace Volte.Core.Services {
    internal class EventService {
        private readonly Log _logger = Log.GetLogger();

        public async Task OnReady() {
            var dbl = VolteBot.Client.GetGuild(264445053596991498);
            if (dbl == null || Config.GetOwner() == 168548441939509248) return;
            await dbl.GetTextChannel(265156286406983680).SendMessageAsync(
                $"<@168548441939509248>: I am a SIVA not owned by you. Please do not post SIVA to a bot list again, <@{Config.GetOwner()}>.");
            await dbl.LeaveAsync();
        }

        public async Task Guilds(SocketGuild guild) {
            if (Config.GetBlacklistedOwners().Contains(guild.OwnerId)) {
                await guild.LeaveAsync();
            }
        }

        
        public async Task OnCommand(Optional<CommandInfo> cinfo, ICommandContext ctx, IResult res) {
            var config = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild);
            if (Config.GetLogAllCommands()) {
                if (res.IsSuccess) {
                    _logger.Info($"--|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator}");
                    _logger.Info($"--|     -Command Issued: {cinfo.Value.Name}");
                    _logger.Info("--|        -Args Passed: " +
                                 ctx.Message.Content.Replace(
                                     $"{config.CommandPrefix}{cinfo.Value.Name} ",
                                     "", StringComparison.CurrentCultureIgnoreCase
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
                                      "", 
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