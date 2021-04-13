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
        public static PollInfo FromFields(params (string Name, object Value)[] fields) => new PollInfo().AddFields(fields);

        public static PollInfo FromDefaultFields(IEnumerable<string> choices)
        {
            var (one, two, three, four, five) = DiscordHelper.GetPollEmojis();
            var collection = choices as string[] ?? choices.ToArray();
            return (collection.Length - 1) switch
            {
                1 => FromFields(($"{one}", collection[1])),

                2 => FromFields(($"{one}", collection[1]),
                    ($"{two}", collection[2])),

                3 => FromFields(($"{one}", collection[1]),
                    ($"{two}", collection[2]),
                    ($"{three}", collection[3])),

                4 => FromFields(($"{one}", collection[1]),
                    ($"{two}", collection[2]),
                    ($"{three}", collection[3]),
                    ($"{four}", collection[4])),

                5 => FromFields(($"{one}", collection[1]),
                    ($"{two}", collection[2]),
                    ($"{three}", collection[3]),
                    ($"{four}", collection[4]),
                    ($"{five}", collection[5])),

                _ => FromInvalid()

            };
        }
        
        public EmbedBuilder Apply(EmbedBuilder embedBuilder) 
            => PollHelper.ApplyPollInfo(embedBuilder, this);

        public static PollInfo FromInvalid()
            => new PollInfo { IsValid = false };

        public Dictionary<string, object> Fields { get; }
        public bool IsValid { get; set; }
        public const string Footer = "Click one of the numbers below to vote.";

        public PollInfo()
        {
            Fields = new Dictionary<string, object>();
            IsValid = true;
        }

        public PollInfo AddFields(params (string Name, object Value)[] fields)
        {
            foreach (var (name, value) in fields)
            {
                if (name.IsNullOrEmpty() || value is null) continue;
                Fields.Add(name, value);
            }

            return this;
        }
    }
}
