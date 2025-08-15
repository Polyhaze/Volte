namespace Volte.Systems.Moderation;

public class AuditLogArchiveMessageBuilder : CustomEmbedBuilder<AuditLogArchiveMessageBuilder>
{
    private readonly SocketTextChannel _channel;

    public bool IsModified { get; private set; }

    public AuditLogArchiveMessageBuilder(SocketUser causer, SocketTextChannel socketTextChannel)
    {
        _channel = socketTextChannel;
        WithAuthor($"Causer: {
            (causer.DiscriminatorValue != 0
                ? $"{causer.Username}#{causer.Discriminator}"
                : causer.Username
            )
        }", causer.GetDisplayAvatarUrl());
    }

    public Task<IUserMessage> SendAsync() => SendToAsync(_channel);

    public override Embed Build() =>
        !IsModified
            ? throw new InvalidOperationException($"Cannot build an unmodified {nameof(AuditLogArchiveMessageBuilder)}") 
            : base.Build();

    public override AuditLogArchiveMessageBuilder WithTitle(string title)
    {
        IsModified = true;

        return base.WithTitle(title);
    }

    public override AuditLogArchiveMessageBuilder WithDescription(string description)
    {
        IsModified = true;

        return base.WithDescription(description);
    }

    public override AuditLogArchiveMessageBuilder WithUrl(string url)
    {
        IsModified = true;

        return base.WithUrl(url);
    }

    public override AuditLogArchiveMessageBuilder WithThumbnailUrl(string thumbnailUrl)
    {
        IsModified = true;

        return base.WithThumbnailUrl(thumbnailUrl);
    }

    public override AuditLogArchiveMessageBuilder WithImageUrl(string imageUrl)
    {
        IsModified = true;

        return base.WithImageUrl(imageUrl);
    }

    public override AuditLogArchiveMessageBuilder WithTimestamp(DateTimeOffset dateTimeOffset)
    {
        IsModified = true;

        return base.WithTimestamp(dateTimeOffset);
    }

    public override AuditLogArchiveMessageBuilder WithFooter(EmbedFooterBuilder footer)
    {
        IsModified = true;

        return base.WithFooter(footer);
    }

    public override AuditLogArchiveMessageBuilder AddField(EmbedFieldBuilder field)
    {
        IsModified = true;

        return base.AddField(field);
    }

    public override AuditLogArchiveMessageBuilder AppendDescription(string description)
    {
        IsModified = true;

        return base.AppendDescription(description);
    }

    public override AuditLogArchiveMessageBuilder AppendDescriptionLine(string description)
    {
        IsModified = true;

        return base.AppendDescriptionLine(description);
    }
}