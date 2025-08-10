using JetBrains.Annotations;

namespace Volte.Entities;

/// <summary>
///     The default <see cref="CustomEmbedBuilder{TSelf}"/> impelementation; with nothing unique about it.
/// </summary>
public sealed class VolteEmbedBuilder : CustomEmbedBuilder<VolteEmbedBuilder>;

/// <summary>
///     A type-safe version of <see cref="EmbedBuilder"/>, with additional utilities previously implemented as an extension on <see cref="EmbedBuilder"/>.
/// </summary>
/// <typeparam name="TSelf">The self-referential type implementing this class.</typeparam>
public abstract class CustomEmbedBuilder<TSelf> : EmbedBuilder where TSelf : CustomEmbedBuilder<TSelf>
{
    #region Builder parts

    private bool _isInInlineBlock;

    /// <summary>
    ///     Sets the title of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="title">The title to be set.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithTitle(string title)
    {
        Title = title;
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the description of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="description"> The description to be set. </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithDescription(string description)
    {
        Description = description;
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the description of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="descriptionBuilder">The description, as a <see cref="StringBuilder"/> configuration delegate.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public TSelf WithDescription(Action<StringBuilder> descriptionBuilder)
    {
        Description = String(descriptionBuilder);
        return (TSelf)this;
    }

    /// <summary>
    ///     Append the given string to the end of the description of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="description"> The description to be appended to the end. </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public TSelf AppendDescription(string description)
    {
        Description += description;
        return (TSelf)this;
    }

    /// <summary>
    ///     Append the given string to the end of the description of an <see cref="Embed"/>, followed by a newline literal.
    /// </summary>
    /// <param name="description"> The description to be appended to the end. </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public TSelf AppendDescriptionLine(string description)
    {
        Description += description;
        Description += '\n';
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the URL of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="url"> The URL to be set. </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithUrl(string url)
    {
        Url = url;
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the thumbnail URL of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="thumbnailUrl"> The thumbnail URL to be set. </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithThumbnailUrl(string thumbnailUrl)
    {
        ThumbnailUrl = thumbnailUrl;
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the image URL of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="imageUrl">The image URL to be set.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithImageUrl(string imageUrl)
    {
        ImageUrl = imageUrl;
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the timestamp of an <see cref="Embed" /> to the current time.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithCurrentTimestamp() => WithTimestamp(DateTimeOffset.UtcNow);

    /// <summary>
    ///     Sets the timestamp of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The timestamp to be set.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithTimestamp(DateTimeOffset dateTimeOffset)
    {
        Timestamp = dateTimeOffset;
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the sidebar color of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="color">The color to be set.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithColor(Color color)
    {
        Color = color;
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the sidebar color of an <see cref="Embed"/> to the success color defined in the config (<see cref="Config.SuccessColor"/>).
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public TSelf WithSuccessColor() => WithColor(Config.SuccessColor);

    /// <summary>
    ///     Sets the sidebar color of an <see cref="Embed"/> to the error color defined in the config (<see cref="Config.ErrorColor"/>).
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public TSelf WithErrorColor() => WithColor(Config.ErrorColor);

    /// <summary>
    ///     Sets the sidebar color of an <see cref="Embed"/> to the user's highest role's color (their name color).
    /// </summary>
    /// <param name="user">The user to get the name color of.</param>
    /// <remarks>If they have no highest role, or their highest role has no color, <see cref="Config.SuccessColor"/> is used.</remarks>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public TSelf WithRelevantColor(SocketGuildUser user)
        => WithColor(user.GetHighestRole()?.Color ?? new Color(Config.SuccessColor));

    /// <summary>
    ///     Sets the <see cref="EmbedAuthorBuilder" /> of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="author">The author builder class containing the author field properties.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithAuthor(EmbedAuthorBuilder author)
    {
        Author = author;
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the author field of an <see cref="Embed" /> with the provided properties.
    /// </summary>
    /// <param name="action">The delegate containing the author field properties.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithAuthor(Action<EmbedAuthorBuilder> action)
    {
        var author = new EmbedAuthorBuilder();
        action(author);
        Author = author;
        return (TSelf)this;
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
    public new TSelf WithAuthor(string name, string iconUrl = null, string url = null)
    {
        var author = new EmbedAuthorBuilder
        {
            Name = name,
            IconUrl = iconUrl,
            Url = url
        };
        Author = author;
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the <see cref="EmbedFooterBuilder" /> of an <see cref="Embed"/>.
    /// </summary>
    /// <param name="footer">The footer builder class containing the footer field properties.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithFooter(EmbedFooterBuilder footer)
    {
        Footer = footer;
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the footer field of an <see cref="Embed" /> with the provided properties.
    /// </summary>
    /// <param name="action">The delegate containing the footer field properties.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithFooter(Action<EmbedFooterBuilder> action)
    {
        var footer = new EmbedFooterBuilder();
        action(footer);
        Footer = footer;
        return (TSelf)this;
    }

    /// <summary>
    ///     Sets the footer field of an <see cref="Embed" /> with the provided name, icon URL.
    /// </summary>
    /// <param name="text">The title of the footer field.</param>
    /// <param name="iconUrl">The icon URL of the footer field.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf WithFooter(string text, string iconUrl = null)
    {
        var footer = new EmbedFooterBuilder
        {
            Text = text,
            IconUrl = iconUrl
        };
        Footer = footer;
        return (TSelf)this;
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
    public TSelf AddField(object name, object value, bool? inline = null)
        => AddField(new EmbedFieldBuilder()
            .WithIsInline(inline ?? _isInInlineBlock)
            .WithName(name.ToString())
            .WithValue(value)
        );

    /// <summary>
    ///     Adds an <see cref="Embed" /> field with the provided name and value.
    /// </summary>
    /// <param name="name">The title of the field.</param>
    /// <param name="valueBuilder">The value of the field, as a <see cref="StringBuilder"/> configuration delegate.</param>
    /// <param name="inline">Indicates whether the field is in-line or not.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public TSelf AddField(object name, Action<StringBuilder> valueBuilder, bool? inline = null)
        => AddField(name, String(valueBuilder), inline);

    /// <summary>
    ///     Adds a field with the provided <see cref="EmbedFieldBuilder" /> to an
    ///     <see cref="Embed"/>.
    /// </summary>
    /// <param name="field">The field builder class containing the field properties.</param>
    /// <exception cref="ArgumentException">Field count exceeds <see cref="EmbedBuilder.MaxFieldCount"/>.</exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf AddField(EmbedFieldBuilder field)
    {
        if (Fields.Count >= MaxFieldCount)
        {
            throw new ArgumentException(message: $"Field count must be less than or equal to {MaxFieldCount}.",
                paramName: nameof(field));
        }

        Fields.Add(field);
        return (TSelf)this;
    }

    /// <summary>
    ///     Adds an <see cref="Embed" /> field with the provided properties.
    /// </summary>
    /// <param name="action">The delegate containing the field properties.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public new TSelf AddField(Action<EmbedFieldBuilder> action)
    {
        var field = new EmbedFieldBuilder
        {
            IsInline = _isInInlineBlock
        };
        action(field);
        AddField(field);
        return (TSelf)this;
    }

    public TSelf EnterInlineFieldBlock()
    {
        _isInInlineBlock = true;
        return (TSelf)this;
    }

    public TSelf ExitInlineFieldBlock()
    {
        _isInInlineBlock = false;
        return (TSelf)this;
    }

    public TSelf AddNullableField<T>(
        string name,
        [CanBeNull] T value,
        Func<T, string> toString = null,
        bool? inline = null
    ) where T : class
    {
        toString ??= it => it.ToString();

        return value is not null
            ? AddField(name, toString(value), inline)
            : (TSelf)this;
    }

    public TSelf AddNullableStringField(
        string name,
        [CanBeNull] string value,
        Func<string, string> transform = null,
        bool? inline = null
    )
    {
        return !string.IsNullOrEmpty(value)
            ? AddField(name, transform is not null ? transform(value) : value, inline)
            : (TSelf)this;
    }

    public TSelf AddNullableField<T>(
        string name,
        T? value,
        Func<T, string> toString = null,
        bool? inline = null
    ) where T : struct
    {
        toString ??= it => it.ToString();

        return value.HasValue
            ? AddField(name, toString(value.Value), inline)
            : (TSelf)this;
    }

    #endregion

    #region Send helpers

    public Task<IUserMessage> SendToAsync(IMessageChannel c) 
        => c.SendMessageAsync(embed: Build(), allowedMentions: AllowedMentions.None);
    
    public Task<IUserMessage> SendToAsync(IGuildUser u) 
        => u.CreateDMChannelAsync().Then(SendToAsync);
    
    public Task<IUserMessage> ReplyToAsync(IUserMessage msg) 
        => msg.ReplyAsync(embed: Build(), allowedMentions: AllowedMentions.None);
    
    public static implicit operator TSelf(CustomEmbedBuilder<TSelf> self) => (TSelf)self;
    public static implicit operator Embed(CustomEmbedBuilder<TSelf> self) => self.Build();

    #endregion
}