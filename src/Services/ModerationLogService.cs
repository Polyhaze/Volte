using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Humanizer;

namespace BrackeysBot.Services
{
    public class ModerationLogService : BrackeysBotService
    {
        private readonly DataService _data;
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _log;

        public ModerationLogService(DataService data, DiscordSocketClient client, LoggingService log)
        {
            _data = data;
            _client = client;
            _log = log;
        }

        public async Task CreateEntry(ModerationLogEntry logEntry, IMessageChannel replyChannel = null)
        {
            if (replyChannel != null)
                await CreateEmbedResponse(logEntry).SendToChannel(replyChannel);

            await _log.LogMessageAsync(new LogMessage(LogSeverity.Info, "ModLog", $"{logEntry.Moderator} performed {logEntry.ActionType}."));
            await PostToLogChannelAsync(logEntry);
        }

        private async Task PostToLogChannelAsync(ModerationLogEntry logEntry)
        {
            ulong moderationLogChannelID = _data.Configuration.ModerationLogChannelID;
            if (moderationLogChannelID == 0)
                throw new Exception("Invalid moderation log channel ID.");

            Embed embed = CreateEmbedLogEntry(logEntry);

            IGuild guild = _client.GetGuild(_data.Configuration.GuildID);
            ITextChannel channel = await guild.GetTextChannelAsync(moderationLogChannelID);
            await embed.SendToChannel(channel);
        }

        private Embed CreateEmbedLogEntry(ModerationLogEntry logEntry)
            => new EmbedBuilder()
                .WithAuthor(logEntry.ActionType.Humanize(), logEntry.Target?.EnsureAvatarUrl())
                .WithColor(GetColorForAction(logEntry.ActionType))
                .AddFieldConditional(logEntry.HasTarget, "User", logEntry.TargetMention, true)
                .AddField("Moderator", logEntry.Moderator.Mention, true)
                .AddFieldConditional(!string.IsNullOrEmpty(logEntry.Reason), "Reason", logEntry.Reason, true)
                .AddFieldConditional(logEntry.Channel != null, "Channel", logEntry.Channel?.Mention, true)
                .AddFieldConditional(logEntry.Duration != null, "Duration", (logEntry.Duration ?? TimeSpan.Zero).Humanize(7), true)
                .AddFieldConditional(logEntry.AdditionalInfo != null, "Additional info", logEntry.AdditionalInfo)
                .WithFooter($"{logEntry.Time.ToTimeString()} | {logEntry.Time.ToDateString()}")
                .Build();
        private Embed CreateEmbedResponse(ModerationLogEntry logEntry)
        {
            EmbedBuilder builder = new EmbedBuilder();

            StringBuilder author = new StringBuilder($"[{logEntry.ActionType.Humanize()}]");
            StringBuilder description = new StringBuilder();

            if (logEntry.HasTarget)
            {
                if (logEntry.Target != null)
                    author.Append($" {logEntry.Target.ToString()}");
                else
                    author.Append($" {logEntry.TargetMention}");
            }
            if (logEntry.Reason != Commands.ModerationModule.DefaultReason)
                description.AppendLine(logEntry.Reason).AppendLine();
            if (logEntry.Duration.HasValue)
                description.AppendLine($"Duration: {logEntry.Duration.Value.Humanize(7)}");

            return builder
                .WithAuthor(author.ToString(), logEntry.Target?.EnsureAvatarUrl())
                .WithColor(GetColorForAction(logEntry.ActionType))
                .WithDescription(description.ToString())
                .Build();
        }

        private Color GetColorForAction(ModerationActionType actionType)
        {
            switch (actionType)
            {
                case ModerationActionType.Ban:
                case ModerationActionType.Mute:
                case ModerationActionType.TempBan:
                case ModerationActionType.TempMute:
                case ModerationActionType.Kick:
                    return Color.Red;

                case ModerationActionType.Unban:
                case ModerationActionType.Unmute:
                    return Color.Green;

                case ModerationActionType.Warn:
                case ModerationActionType.DeletedInfraction:
                case ModerationActionType.ClearInfractions:
                    return Color.Orange;

                case ModerationActionType.ClearMessages:
                case ModerationActionType.Filtered:
                    return Color.LightOrange;

                default:
                    return new Color(0, 0, 0);
            }
        }
    }
}
