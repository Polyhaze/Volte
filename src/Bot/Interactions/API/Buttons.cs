// ReSharper disable MemberCanBePrivate.Global UnusedMember.Global
namespace Volte.Interactions;

public static class Buttons
    {
        private static ButtonBuilder CreateBuilder(MessageComponentId id, ButtonStyle style, IEmote emote = null) 
            => new ButtonBuilder().WithStyle(style).WithCustomId(id).WithEmote(emote);

        private static void EnsureNeitherNull(string label, IEmote emote)
        {
            if (label is null && emote is null)
                throw new ArgumentException("Cannot create a button without a label OR emote.");
        }
        
        public static ButtonBuilder Danger(MessageComponentId id, string label = null, IEmote emote = null, bool disabled = false)
        {
            EnsureNeitherNull(label, emote);

            return (label is null
                ? CreateBuilder(id, ButtonStyle.Danger, emote).WithDisabled(disabled)
                : ButtonBuilder.CreateDangerButton(label, id,  emote)).WithDisabled(disabled);
        }
        
        public static ButtonBuilder Success(MessageComponentId id, string label = null, IEmote emote = null, bool disabled = false)
        {
            EnsureNeitherNull(label, emote);

            return (label is null
                ? CreateBuilder(id, ButtonStyle.Success, emote).WithDisabled(disabled)
                : ButtonBuilder.CreateSuccessButton(label, id,  emote)).WithDisabled(disabled);
        }
        
        public static ButtonBuilder Primary(MessageComponentId id, string label = null, IEmote emote = null, bool disabled = false)
        {
            EnsureNeitherNull(label, emote);

            return (label is null
                ? CreateBuilder(id, ButtonStyle.Primary, emote).WithDisabled(disabled)
                : ButtonBuilder.CreatePrimaryButton(label, id, emote)).WithDisabled(disabled);
        } 
        
        public static ButtonBuilder Secondary(MessageComponentId id, string label = null, IEmote emote = null, bool disabled = false)
        {
            EnsureNeitherNull(label, emote);

            return (label is null
                ? CreateBuilder(id, ButtonStyle.Secondary, emote).WithDisabled(disabled)
                : ButtonBuilder.CreateSecondaryButton(label, id,  emote)).WithDisabled(disabled);
        }

        public static ButtonBuilder Link(string url, string label, IEmote emote = null, bool disabled = false) =>
            ButtonBuilder.CreateLinkButton(label, url, emote).WithDisabled(disabled);
    }

#nullable enable
    /// <summary>
    ///     Over-engineered wrapper around <see cref="IMessageComponent"/> custom IDs.
    ///     This class and its methods (minus the constructor allowing for a raw string ID)
    ///     assume the custom ID follows the form <code>identifier:action:value:trailingContent</code>
    ///     Considering each part is just a string, the names don't matter much, but it provides IntelliSense for button ID checking.
    /// </summary>
    public class MessageComponentId
    {
        /// <summary>
        ///     The character that separates the ID.
        /// </summary>
        public const char Separator = ':';
        
        /// <summary>
        ///     The identifier (the first part) of the Custom ID.
        /// </summary>
        public string Identifier { get; } = null!;

        /// <summary>
        ///     The action (the second part) of the Custom ID.
        /// </summary>
        public string Action { get; } = null!;

        /// <summary>
        ///     The value (the third part) of the Custom ID.
        /// </summary>
        public string? Value { get; }

        /// <summary>
        ///     The trailing content of the Custom ID (everything after the semicolon after the value content, if present).
        /// </summary>
        public string? TrailingContent { get; }

        private StringBuilder? _rawId;

        /// <summary>
        ///     Create a new <see cref="MessageComponentId"/> with <paramref name="raw"/> as its content.
        /// </summary>
        /// <param name="raw">The raw ID of this component.</param>
        public MessageComponentId(string raw)
        {
            _rawId = new StringBuilder(raw);
            var split = raw.Split(Separator);
            if (split.Length < 3) return;

            Identifier = split[0];
            Action = split[1];
            Value = split.ElementAtOrDefault(2);
            
            TrailingContent = raw.Length == raw.LastIndexOf(Separator) + 1
                ? null
                : raw[(raw.LastIndexOf(Separator) + 1)..];
        }
        
        /// <summary>
        ///     Create a new <see cref="MessageComponentId"/>.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="action"></param>
        /// <param name="value"></param>
        /// <param name="trailing"></param>
        public MessageComponentId(object identifier, object action, object? value, object? trailing)
        {
            Guard.Require(identifier, nameof(identifier));
            Guard.Require(action, nameof(action));
            
            Identifier = identifier.ToString()!;
            Action = action.ToString()!;
            Value = value?.ToString();
            TrailingContent = trailing?.ToString();
        }

        public static implicit operator MessageComponentId(string raw) => new(raw);

        public static implicit operator MessageComponentId((object, object, object, object) segments)
            => new(segments.Item1, segments.Item2, segments.Item3, segments.Item4);

        public static implicit operator string(MessageComponentId id) => id.ToString();

        public override string ToString()
        {
            // ReSharper disable once InvertIf
            if (_rawId == null)
            {
                _rawId = new StringBuilder();
                AppendSegment(Identifier);
                AppendSegment(Action);
                if (Value != null)
                    AppendSegment(Value);
                if (TrailingContent != null)
                    _rawId.Append(TrailingContent);
            }
            
            return _rawId.ToString();
        }

        private void AppendSegment(object val) 
            => _rawId?.Append($"{val}{Separator}");
    }