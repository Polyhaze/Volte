using System.Collections.Generic;
using System.Linq;
using Discord;
using Gommon;
using Volte.Core.Helpers;

// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Core.Entities
{
    public sealed class PollInfo
    {
        public static PollInfo FromFields(params (object Name, object Value)[] fields) => new PollInfo().AddFields(fields);

        public static PollInfo FromDefaultFields(IEnumerable<string> choices)
        {
            var (one, two, three, four, five) = DiscordHelper.GetPollButtons();
            var collection = choices as string[] ?? choices.ToArray();
            return (collection.Length - 1) switch
            {
                1 => FromFields((one, collection[1])),
                2 => FromFields((one, collection[1]),
                    (two, collection[2])),
                3 => FromFields((one, collection[1]),
                    (two, collection[2]),
                    (three, collection[3])),
                4 => FromFields((one, collection[1]),
                    (two, collection[2]),
                    (three, collection[3]),
                    (four, collection[4])),
                5 => FromFields((one, collection[1]),
                    (two, collection[2]),
                    (three, collection[3]),
                    (four, collection[4]),
                    (five, collection[5])),
                _ => FromInvalid(collection.Length > 6 ? "More than 5 options specified." : "No options specified.")
            };
        }
        
        public EmbedBuilder Apply(EmbedBuilder embedBuilder) 
            => PollHelper.ApplyPollInfo(embedBuilder, this);

        public static PollInfo FromInvalid(string reason)
            => new PollInfo
            {
                Validation = (false, reason)
            };

        public PollInfo WithPrompt(string prompt)
        {
            Prompt = prompt;
            return this;
        }

        public string Prompt { get; set; }
        public Dictionary<string, object> Fields { get; }
        public (bool IsValid, string InvalidationReason) Validation { get; set; }
        public const string Footer = "Click one of the numbers below to vote.";

        public PollInfo()
        {
            Fields = new Dictionary<string, object>();
            Validation = (true, null);
        }

        public PollInfo AddFields(params (object Name, object Value)[] fields)
        {
            foreach (var (name, value) in fields.Select(x => (x.Name.ToString(), x.Value.ToString())))
            {
                if (name.IsNullOrEmpty() || value is null) continue;
                Fields.Add(name, value);
            }

            return this;
        }
    }
}
