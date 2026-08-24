using System.Collections.Concurrent;
using ActionType = Volte.Systems.UserFilter.ActionType;

namespace Volte.Systems.AttachmentSpam;

public class AttachmentSpamService : VolteService
{
    private static readonly TimeSpan SpamThreshold = TimeSpan.FromSeconds(11);

    private readonly ConcurrentDictionary<ulong, AttachmentEvidence> _lastAttachmentByUser = new();
    private readonly DiscordSocketClient _client;
    private readonly ModerationService _moderationService;

    public AttachmentSpamService(DiscordSocketClient client, ModerationService moderationService)
    {
        _client = client;
        _moderationService = moderationService;
    }

    public async ValueTask<bool> MessageReceivedAsync(MessageReceivedEventArgs args)
    {
        if (args.Context.Message.Channel is not SocketTextChannel textChannel ||
            !args.Context.GuildData.Settings.Moderation.AttachmentSpamDetection)
        {
            return false;
        }

        var userId = args.Message.Author.Id;
        if (args.Message.Attachments.Count < 2)
        {
            _lastAttachmentByUser.TryRemove(userId, out _);
            return false;
        }

        var currentEvidence = new AttachmentEvidence(
            textChannel.Id,
            textChannel.Name,
            args.Message.Content,
            args.Message.GetJumpUrl(),
            [.. args.Message.Attachments.Select(a => a.Url)],
            DateTimeOffset.UtcNow
        );

        if (_lastAttachmentByUser.TryGetValue(userId, out var previousEvidence) &&
            previousEvidence.ChannelId != currentEvidence.ChannelId &&
            currentEvidence.CreatedAt - previousEvidence.CreatedAt <= SpamThreshold)
        {
            await HandleAttachmentSpamAsync(args, previousEvidence, currentEvidence);
            _lastAttachmentByUser.TryRemove(userId, out _);
            return true;
        }

        _lastAttachmentByUser[userId] = currentEvidence;
        return false;
    }

    private async Task HandleAttachmentSpamAsync(
        MessageReceivedEventArgs triggeringEvent,
        AttachmentEvidence firstEvidence,
        AttachmentEvidence secondEvidence)
    {
        try
        {
            var guildUser = triggeringEvent.Context.Guild.GetUser(triggeringEvent.Message.Author.Id);
            if (guildUser is null)
            {
                return;
            }

            if (await ExecuteAsync(
                    triggeringEvent.Context.GuildData.Settings.Moderation.AttachmentSpamAction,
                    triggeringEvent.Context.GuildData.Settings.Moderation.AttachmentSpamReason,
                    triggeringEvent.Context.Guild,
                    guildUser
                ))
            {
                await _moderationService.OnModActionCompleteAsync(new ModActionEventArgs
                    {
                        CreateEmbedBuilder = triggeringEvent.Context.CreateEmbedBuilder,
                        GuildData = triggeringEvent.Context.GuildData
                    }
                    .WithActionType(ModActionType.AttachmentSpam)
                    .WithModerator(_client.CurrentUser)
                    .WithTarget(guildUser)
                    .WithGuild(triggeringEvent.Context.Guild)
                    .WithReason(triggeringEvent.Context.GuildData.Settings.Moderation.AttachmentSpamReason)
                    .WithTime(secondEvidence.CreatedAt)
                    .WithAttachmentEvidences(firstEvidence, secondEvidence)
                );
            }
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }

    public async Task<bool> ExecuteAsync(ActionType type, string reason, IGuild guild, IGuildUser user)
    {
        switch (type)
        {
            case ActionType.Kick:
                try
                {
                    await user.KickAsync(reason,
                        DiscordHelper.RequestOptions(x => x.AuditLogReason = "Triggered attachment spam detection"));
                    Debug(LogSource.Service,
                        $"Kicked {user} in guild {user.Guild.Id} because they triggered attachment spam detection");
                    return true;
                }
                catch (HttpException)
                {
                    return false;
                }
            case ActionType.Ban:
                try
                {
                    await guild.AddBanAsync(user.Id, 7, reason,
                        DiscordHelper.RequestOptions(x => x.AuditLogReason = "Triggered attachment spam detection"));
                    Debug(LogSource.Service,
                        $"Banned {user} in guild {user.Guild.Id} because they triggered attachment spam detection");
                    return true;
                }
                catch (HttpException)
                {
                    return false;
                }
            case ActionType.SoftBan:
                try
                {
                    await guild.AddBanAsync(user.Id, 7, reason,
                        DiscordHelper.RequestOptions(x => x.AuditLogReason = "Triggered attachment spam detection"));
                    await guild.RemoveBanAsync(user.Id);
                    Debug(LogSource.Service,
                        $"Softbanned {user} in guild {user.Guild.Id} because they triggered attachment spam detection");
                    return true;
                }
                catch (HttpException)
                {
                    return false;
                }
            default:
                Debug(LogSource.Service,
                    $"Unknown or unsupported action type {Enum.GetName(type)} for attachment spam detection for guild {guild.Id}");
                return false;
        }
    }
}