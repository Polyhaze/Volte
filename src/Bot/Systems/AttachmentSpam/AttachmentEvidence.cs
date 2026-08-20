namespace Volte.Systems.AttachmentSpam;

public sealed record AttachmentEvidence(
    ulong ChannelId,
    string ChannelName,
    string Content,
    string JumpUrl,
    IReadOnlyList<string> AttachmentUrls,
    DateTimeOffset CreatedAt
);