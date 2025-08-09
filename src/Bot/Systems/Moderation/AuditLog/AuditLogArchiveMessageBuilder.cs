using JetBrains.Annotations;

namespace Volte.Systems.Moderation;

public class AuditLogArchiveMessageBuilder : EmbedBuilder
{
    private readonly SocketTextChannel _channel;

    private bool _isInInlineBlock;

    public AuditLogArchiveMessageBuilder(SocketUser causer, SocketTextChannel socketTextChannel)
    {
        _channel = socketTextChannel;
        this.WithAuthor(causer);
    }

    /// <summary>
    ///     Sets the title of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="title">The title to be set.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    ///     Sets the description of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="description"> The description to be set. </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithDescription(string description)
    {
        Description = description;
        return this;
    }

    /// <summary>
    ///     Sets the URL of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="url"> The URL to be set. </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithUrl(string url)
    {
        Url = url;
        return this;
    }

    /// <summary>
    ///     Sets the thumbnail URL of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="thumbnailUrl"> The thumbnail URL to be set. </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithThumbnailUrl(string thumbnailUrl)
    {
        ThumbnailUrl = thumbnailUrl;
        return this;
    }

    /// <summary>
    ///     Sets the image URL of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="imageUrl">The image URL to be set.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithImageUrl(string imageUrl)
    {
        ImageUrl = imageUrl;
        return this;
    }

    /// <summary>
    ///     Sets the timestamp of an <see cref="Embed" /> to the current time.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithCurrentTimestamp()
    {
        Timestamp = DateTimeOffset.UtcNow;
        return this;
    }

    /// <summary>
    ///     Sets the timestamp of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The timestamp to be set.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithTimestamp(DateTimeOffset dateTimeOffset)
    {
        Timestamp = dateTimeOffset;
        return this;
    }

    /// <summary>
    ///     Sets the sidebar color of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="color">The color to be set.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithColor(Color color)
    {
        Color = color;
        return this;
    }

    /// <summary>
    ///     Sets the sidebar color of an <see cref="Embed"/> to the success color in the config file.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public AuditLogArchiveMessageBuilder WithSuccessColor() => WithColor(Config.SuccessColor);

    /// <summary>
    ///     Sets the <see cref="EmbedAuthorBuilder" /> of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="author">The author builder class containing the author field properties.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithAuthor(EmbedAuthorBuilder author)
    {
        Author = author;
        return this;
    }

    /// <summary>
    ///     Sets the author field of an <see cref="Embed" /> with the provided properties.
    /// </summary>
    /// <param name="action">The delegate containing the author field properties.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithAuthor(Action<EmbedAuthorBuilder> action)
    {
        var author = new EmbedAuthorBuilder();
        action(author);
        Author = author;
        return this;
    }

    /// <summary>
    ///     Sets the author field of an <see cref="Embed" /> with the provided name, icon URL, and URL.
    /// </summary>
    /// <param name="name">The title of the author field.</param>
    /// <param name="iconUrl">The icon URL of the author field.</param>
    /// <param name="url">The URL of the author field.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithAuthor(string name, string iconUrl = null, string url = null)
    {
        var author = new EmbedAuthorBuilder
        {
            Name = name,
            IconUrl = iconUrl,
            Url = url
        };
        Author = author;
        return this;
    }

    /// <summary>
    ///     Sets the <see cref="EmbedFooterBuilder" /> of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="footer">The footer builder class containing the footer field properties.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithFooter(EmbedFooterBuilder footer)
    {
        Footer = footer;
        return this;
    }

    /// <summary>
    ///     Sets the footer field of an <see cref="Embed" /> with the provided properties.
    /// </summary>
    /// <param name="action">The delegate containing the footer field properties.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithFooter(Action<EmbedFooterBuilder> action)
    {
        var footer = new EmbedFooterBuilder();
        action(footer);
        Footer = footer;
        return this;
    }

    /// <summary>
    ///     Sets the footer field of an <see cref="Embed" /> with the provided name, icon URL.
    /// </summary>
    /// <param name="text">The title of the footer field.</param>
    /// <param name="iconUrl">The icon URL of the footer field.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder WithFooter(string text, string iconUrl = null)
    {
        var footer = new EmbedFooterBuilder
        {
            Text = text,
            IconUrl = iconUrl
        };
        Footer = footer;
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="Embed" /> field with the provided name and value.
    /// </summary>
    /// <param name="name">The title of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="inline">Indicates whether the field is in-line or not.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public AuditLogArchiveMessageBuilder AddField(string name, object value, bool? inline = null)
    {
        var field = new EmbedFieldBuilder()
            .WithIsInline(inline ?? _isInInlineBlock)
            .WithName(name)
            .WithValue(value);
        AddField(field);
        return this;
    }

    /// <summary>
    ///     Adds a field with the provided <see cref="EmbedFieldBuilder" /> to an
    ///     <see cref="Embed"/>.
    /// </summary>
    /// <param name="field">The field builder class containing the field properties.</param>
    /// <exception cref="ArgumentException">Field count exceeds <see cref="EmbedBuilder.MaxFieldCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder AddField(EmbedFieldBuilder field)
    {
        if (Fields.Count >= MaxFieldCount)
        {
            throw new ArgumentException(message: $"Field count must be less than or equal to {MaxFieldCount}.",
                paramName: nameof(field));
        }

        Fields.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="Embed" /> field with the provided properties.
    /// </summary>
    /// <param name="action">The delegate containing the field properties.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new AuditLogArchiveMessageBuilder AddField(Action<EmbedFieldBuilder> action)
    {
        var field = new EmbedFieldBuilder
        {
            IsInline = _isInInlineBlock
        };
        action(field);
        AddField(field);
        return this;
    }

    public AuditLogArchiveMessageBuilder EnterInlineFieldBlock()
    {
        _isInInlineBlock = true;
        return this;
    }

    public AuditLogArchiveMessageBuilder ExitInlineFieldBlock()
    {
        _isInInlineBlock = false;
        return this;
    }

    public AuditLogArchiveMessageBuilder AddNullableField<T>(
        string name,
        [CanBeNull] T value,
        Func<T, string> toString = null,
        bool? inline = null
    ) where T : class
    {
        toString ??= it => it.ToString();

        return value is not null 
            ? AddField(name, toString(value), inline) 
            : this;
    }

    public AuditLogArchiveMessageBuilder AddNullableStringField(
        string name,
        [CanBeNull] string value,
        Func<string, string> transform = null,
        bool? inline = null
    )
    {
        return !string.IsNullOrEmpty(value) 
            ? AddField(name, transform is not null ? transform(value) : value, inline) 
            : this;
    }

    public AuditLogArchiveMessageBuilder AddNullableField<T>(
        string name,
        T? value,
        Func<T, string> toString = null,
        bool? inline = null
    ) where T : struct
    {
        toString ??= it => it.ToString();

        return value.HasValue 
            ? AddField(name, toString(value.Value), inline) 
            : this;
    }

#pragma warning disable CA1822
    public Task Done() => Task.CompletedTask;
#pragma warning restore CA1822

    public Task<IUserMessage> SendAsync() => this.SendToAsync(_channel);
}