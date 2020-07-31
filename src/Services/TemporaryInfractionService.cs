using System;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

using Discord;
using Discord.WebSocket;

namespace BrackeysBot.Services
{
    public class TemporaryInfractionService : BrackeysBotService, IInitializeableService
    {
        private readonly DataService _data;
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _log;
        private readonly ModerationLogService _modLog;

        private Timer _checkTimer;

        public TemporaryInfractionService(DataService data, DiscordSocketClient client, LoggingService log, ModerationLogService modLog)
        {
            _data = data;
            _client = client;
            _log = log;
            _modLog = modLog;
        }
        public void Initialize()
        {
            _checkTimer = new Timer(TimeSpan.FromSeconds(20).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };
            _checkTimer.Elapsed += (s, e) => CheckTemporaryInfractions();
            _checkTimer.Start();

            _client.UserJoined += CheckUserMuted;
        }

        private void CheckTemporaryInfractions()
        {
            DateTime now = DateTime.UtcNow;
            SocketGuild guild = _client.GetGuild(_data.Configuration.GuildID);

            int resolvedCounter = 0;

            foreach (UserData user in _data.UserData.GetUsersWithTemporaryInfractions())
            {
                if (user.HasTemporaryInfraction(TemporaryInfractionType.TempBan))
                {
                    TemporaryInfraction infraction = user.TemporaryInfractions.First(t => t.Type == TemporaryInfractionType.TempBan);
                    if (infraction.Expire <= now)
                    {
                        guild.RemoveBanAsync(user.ID);

                        _ = _modLog.CreateEntry(ModerationLogEntry.New
                            .WithActionType(ModerationActionType.Unban)
                            .WithTarget(user.ID)
                            .WithReason("Temporary ban timed out.")
                            .WithTime(DateTimeOffset.Now)
                            .WithModerator(_client.CurrentUser));

                        user.TemporaryInfractions.RemoveAll(i => i.Type == TemporaryInfractionType.TempBan);
                        resolvedCounter++;
                    }
                }
                if (user.HasTemporaryInfraction(TemporaryInfractionType.TempMute))
                {
                    TemporaryInfraction infraction = user.TemporaryInfractions.First(t => t.Type == TemporaryInfractionType.TempMute);
                    if (infraction.Expire <= now)
                    {
                        IRole mutedRole = guild.GetRole(_data.Configuration.MutedRoleID);

                        // If the user is no longer in the server, just remove the entry, but don't attempt to remove his role.
                        IGuildUser guildUser = guild.GetUser(user.ID);
                        guildUser?.RemoveRoleAsync(mutedRole);

                        user.Muted = false;
                        _data.SaveUserData();

                        ModerationLogEntry entry = ModerationLogEntry.New
                            .WithActionType(ModerationActionType.Unmute)
                            .WithReason("Temporary mute timed out.")
                            .WithTime(DateTimeOffset.Now)
                            .WithModerator(_client.CurrentUser);

                        if (guildUser != null)
                            entry = entry.WithTarget(guildUser);
                        else
                            entry = entry.WithTarget(user.ID);

                        _ = _modLog.CreateEntry(entry);

                        user.TemporaryInfractions.RemoveAll(i => i.Type == TemporaryInfractionType.TempMute);
                        resolvedCounter++;
                    }
                }
            }

            if (resolvedCounter > 0)
            {
                _log.LogMessageAsync(new LogMessage(LogSeverity.Info, "TemporaryInfractions", $"Resolved {resolvedCounter} temporary infraction(s)."));
                _data.SaveUserData();
            }
        }
        private async Task CheckUserMuted(SocketGuildUser user)
        {
            if (_data.UserData.HasUser(user.Id))
            {
                UserData data = _data.UserData.GetUser(user.Id);
                
                if (HasTemporaryMute(data))
                {
                    TemporaryInfraction infraction = data.TemporaryInfractions.First(t => t.Type == TemporaryInfractionType.TempMute);

                    if (infraction.Expire > DateTime.UtcNow)
                        await MuteUser(user, true);
                }
                else if (data.Muted) 
                {
                    await MuteUser(user, false);
                }
            }
        }

        private bool HasTemporaryMute(UserData data) 
        => data.TemporaryInfractions != null && data.TemporaryInfractions.Any(t => t.Type == TemporaryInfractionType.TempMute);

        private async Task MuteUser(SocketGuildUser user, bool temporary) 
        {
            IRole mutedRole = _client.GetGuild(_data.Configuration.GuildID).GetRole(_data.Configuration.MutedRoleID);
            await user.AddRoleAsync(mutedRole);

            if (temporary)
            {
                await _log.LogMessageAsync(new LogMessage(LogSeverity.Info, "TemporaryInfractions", 
                    $"Muted {user.ToString()} ({user.Id}) since their temporary mute has not expired yet."));
            }
            else 
            {
                await _log.LogMessageAsync(new LogMessage(LogSeverity.Info, "Infractions", 
                    $"Muted {user.ToString()} ({user.Id})"));
            }
        }
    }
}
