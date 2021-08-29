using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Services
{
    public class ReminderService : IVolteService
    {
        private static readonly Regex JumpUrl =
            new Regex(
                @"https?://(?:(?:ptb|canary)\.)?discord(app)?\.com/channels/(?<GuildId>.+)/(?<ChannelId>\d+)/(?<MessageId>\d+)/?",
                RegexOptions.Compiled);

        private static readonly TimeSpan PollRate = 30.Seconds();
        private static Timer _checker;
        private readonly DatabaseService _db;
        private readonly DiscordShardedClient _client;

        public ReminderService(DatabaseService databaseService,
            DiscordShardedClient client)
        {
            _client = client;
            _db = databaseService;
        }

        /// <summary>
        ///     Sets the value of private static field <see cref="_checker"/>.
        ///     If its value is already set; this method returns immediately.
        /// </summary>
        public void Initialize()
        {
            _checker ??= new Timer(
                _ => Executor.Execute(() =>
                {
                    Logger.Debug(LogSource.Service, "Checking all reminders.");
                    _db.GetAllReminders().ForEachIndexedAsync(async (reminder, index) =>
                    {
                        Logger.Debug(LogSource.Service,
                            $"Reminder '{reminder.ReminderText}', set for {reminder.TargetTime} at index {index}");
                        if (reminder.TargetTime.Ticks <= DateTime.Now.Ticks)
                            await SendAsync(reminder);
                    });
                }),
                null,
                5.Seconds(),
                PollRate
            );
        }

        private async Task SendAsync(Reminder reminder)
        {
            var author = _client.GetUser(reminder.CreatorId);
            if (author is null)
            {
                if (_db.TryDeleteReminder(reminder))
                    Logger.Debug(LogSource.Service,
                        "Reminder deleted from the database as its creator .");
                Logger.Debug(LogSource.Service, "Reminder's creator was no longer present in the guild; aborting.");
                return;
            }
            
            var timestamp = reminder.CreationTime.GetDiscordTimestamp(TimestampType.Relative);

            await new EmbedBuilder()
                .WithTitle("Reminder")
                .WithSuccessColor()
                .WithDescription(IsMessageUrl(reminder)
                    ? $"You asked me {timestamp} to remind you about {Format.Url("this message", reminder.ReminderText)}."
                    : $"You asked me {timestamp} to remind you about:\n{"-".Repeat(50)}\n\n {reminder.ReminderText}")
                .SendToAsync(author);
            _db.TryDeleteReminder(reminder);
        }

        private bool IsMessageUrl(Reminder reminder) => JumpUrl.IsMatch(reminder.ReminderText, out var match) &&
                                                        (match.Groups["GuildId"].Value is "@me" ||
                                                         ulong.TryParse(match.Groups["GuildId"].Value, out _));
    }
}