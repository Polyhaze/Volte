using System.Text.RegularExpressions;
using Volte.Systems.Database;

namespace Volte.Systems.Reminder;

public sealed partial class ReminderService : VolteService, IDisposable
{
    private static readonly Regex JumpUrl = MessageUrlPattern();
    
    private readonly PeriodicTimer _ticker = new(30.Seconds());
    private readonly CancellationTokenSource _tickerTokenSource = new();
    private readonly DatabaseService _db;
    private readonly DiscordSocketClient _client;

    public ReminderService(
        DatabaseService db,
        DiscordSocketClient client)
    {
        _db = db;
        _client = client;
    }

    public void Initialize() =>
        ExecuteBackgroundAsync(async () =>
        {
            while (await _ticker.WaitForNextTickAsync(_tickerTokenSource.Token))
            {
                Debug(LogSource.Service, "Checking all reminders.");
                var currentTicks = DateTime.Now.Ticks;
                foreach (var (reminder, index) in _db.GetAllReminders().WithIndex()) 
                {
                    Debug(LogSource.Service,
                        $"Reminder '{reminder.ReminderText}', set for {reminder.TargetTime} at index {index}");
                    if (reminder.TargetTime.Ticks <= currentTicks) 
                        await SendAsync(reminder);
                }
            }
        });

    private async Task SendAsync(Reminder reminder)
    {
        var guild = _client.GetGuild(reminder.GuildId);
        var channel = guild?.GetTextChannel(reminder.ChannelId);
        if (channel is null)
        {
            if (_db.TryDeleteReminder(reminder))
                Debug(LogSource.Service,
                    "Reminder deleted from the database as Volte no longer has access to the channel it was created in.");
            Debug(LogSource.Service,
                "Reminder's target channel was no longer accessible in the guild; aborting.");
            return;
        }

        var author = guild.GetUser(reminder.CreatorId);
        if (author is null)
        {
            if (_db.TryDeleteReminder(reminder))
                Debug(LogSource.Service,
                    "Reminder deleted from the database as its creator is no longer in the guild it was made.");
            Debug(LogSource.Service, "Reminder's creator was no longer present in the guild; aborting.");
            return;
        }

        var timestamp = reminder.CreationTime.ToDiscordTimestamp(TimestampType.Relative);
        var timestampStr = (await channel.GetMessageAsync(reminder.MessageId).AsOptional())
            .Convert(msg => Format.Url(timestamp, msg.GetJumpUrl()))
            .OrElse(timestamp);

        await channel.SendMessageAsync(author.Mention, embed: new EmbedBuilder()
            .WithTitle("Reminder")
            .WithRelevantColor(author)
            .WithDescription(IsMessageUrl(reminder)
                ? $"You asked me {timestampStr} to remind you about {Format.Url("this message", reminder.ReminderText)}."
                : $"You asked me {timestampStr} to remind you about:\n{new string('-', 20)}\n{reminder.ReminderText}")
            .Build());
        _db.TryDeleteReminder(reminder);
    }

    private static bool IsMessageUrl(Reminder reminder) => JumpUrl.IsMatch(reminder.ReminderText, out var match) &&
                                                           (match.Groups["GuildId"].Value is "@me" ||
                                                            match.Groups["GuildId"].Value.TryParse<ulong>(out _));
        
    [GeneratedRegex(@"https?://(?:(?:ptb|canary)\.)?discord(app)?\.com/channels/(?<GuildId>.+)/(?<ChannelId>\d+)/(?<MessageId>\d+)/?", RegexOptions.Compiled)]
    private static partial Regex MessageUrlPattern();

    public void Dispose()
    {
        _tickerTokenSource?.Cancel();
            
        _ticker?.Dispose();
        _tickerTokenSource?.Dispose();
    }
}