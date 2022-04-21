using System;
using System.Linq;
using System.Text;
using Discord;
using Gommon;

namespace Volte.Interactions
{
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
                ? CreateBuilder(id, ButtonStyle.Danger, emote)
                : ButtonBuilder.CreateDangerButton(id, label, emote)).WithDisabled(disabled);
        }
        
        public static ButtonBuilder Success(MessageComponentId id, string label = null, IEmote emote = null, bool disabled = false)
        {
            EnsureNeitherNull(label, emote);

            return (label is null
                ? new ButtonBuilder()
                    .WithStyle(ButtonStyle.Success)
                    .WithCustomId(id)
                    .WithEmote(emote)
                : ButtonBuilder.CreateSuccessButton(id, label, emote)).WithDisabled(disabled);
        }
        
        public static ButtonBuilder Primary(MessageComponentId id, string label = null, IEmote emote = null, bool disabled = false)
        {
            EnsureNeitherNull(label, emote);

            return (label is null
                ? new ButtonBuilder()
                    .WithStyle(ButtonStyle.Primary)
                    .WithCustomId(id)
                    .WithEmote(emote)
                : ButtonBuilder.CreatePrimaryButton(id, label, emote)).WithDisabled(disabled);
        } 
        
        public static ButtonBuilder Secondary(MessageComponentId id, string label = null, IEmote emote = null, bool disabled = false)
        {
            EnsureNeitherNull(label, emote);

            return (label is null
                ? new ButtonBuilder()
                    .WithStyle(ButtonStyle.Secondary)
                    .WithCustomId(id)
                    .WithEmote(emote)
                : ButtonBuilder.CreateSecondaryButton(id, label, emote)).WithDisabled(disabled);
        }

        public static ButtonBuilder Link(string url, string label, IEmote emote = null, bool disabled = false) =>
            ButtonBuilder.CreateLinkButton(label, url, emote).WithDisabled(disabled);
    }

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

        private object _identifier;
        private object _action;
        private object _value;
        private object _trailing;

        private StringBuilder _rawId;

        /// <summary>
        ///     Create a new <see cref="MessageComponentId"/> with <paramref name="raw"/> as its content.
        /// </summary>
        /// <param name="raw">The raw ID of this component.</param>
        public MessageComponentId(string raw)
        {
            _rawId = new StringBuilder(raw);
            var split = raw.Split(Separator);
            if (!split.IsEmpty())
            {
                _identifier = Lambda.TryOrNull(() => split.First());
                _action = Lambda.TryOrNull(() => split[1]);
                _value = Lambda.TryOrNull(() => split[2]);
                _trailing = Lambda.TryOrNull(() => raw[(raw.LastIndexOf(Separator) + 1)..]);
            }
        }
        
        /// <summary>
        ///     Create a new <see cref="MessageComponentId"/>.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="action"></param>
        /// <param name="value"></param>
        /// <param name="trailing"></param>
        public MessageComponentId(object identifier, object action, object value, object trailing)
        {
            _identifier = identifier;
            _action = action;
            _value = value;
            _trailing = trailing;
        }

        public static implicit operator MessageComponentId(string raw) => new MessageComponentId(raw);

        public static implicit operator MessageComponentId((object, object, object, object) segments)
            => new MessageComponentId(segments.Item1, segments.Item2, segments.Item3, segments.Item4);

        public static implicit operator string(MessageComponentId id) => id.ToString();

        public override string ToString()
        {
            if (_rawId != null)
                return _rawId.ToString();

            _rawId = new StringBuilder();
            if (_identifier != null)
                AppendSegment(_identifier);
            if (_action != null)
                AppendSegment(_action);
            if (_value != null)
                AppendSegment(_value);
            if (_trailing != null)
                _rawId.Append(_trailing);

            var result = _rawId.ToString();
            _rawId = null;
            return result;
        }

        private void AppendSegment(object val) => _rawId.Append($"{val}{Separator}");
        
        /// <summary>
        ///     The identifier (the first part) of the Custom ID.
        /// </summary>
        public string Identifier
        {
            get => Lambda.TryOrNull(() => _identifier.ToString());
            set => _identifier = value;
        }

        /// <summary>
        ///     The action (the second part) of the Custom ID.
        /// </summary>
        public string Action
        {
            get => Lambda.TryOrNull(() => _action.ToString());
            set => _action = value;
        }
        
        /// <summary>
        ///     The value (the third part) of the Custom ID.
        /// </summary>
        public string Value
        {
            get => Lambda.TryOrNull(() => _value.ToString());
            set => _value = value;
        }

        /// <summary>
        ///     The trailing content of the Custom ID.
        /// </summary>
        public string TrailingContent
        {
            get => Lambda.TryOrNull(() => _trailing.ToString());
            set => _trailing = value;
        }
    }
}