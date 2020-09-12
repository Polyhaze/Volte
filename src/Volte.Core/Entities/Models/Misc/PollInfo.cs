using System.Collections.Generic;
using System.Linq;
using Gommon;
using JetBrains.Annotations;
using Volte.Core.Helpers;

// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Core.Models.Misc
{
    public sealed class PollInfo
    {
        [NotNull]
        public static PollInfo FromFields(params (string Name, string Value)[] fields) 
            => new PollInfo().AddFields(fields);

        [NotNull]
        public static PollInfo FromDefaultFields(int count, IEnumerable<string> choices)
        {
            var (one, two, three, four, five) = EmojiHelper.GetPollEmojis();
            var collection = choices as string[] ?? choices.ToArray();
            return count switch
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

                _ => FromValid(false)
                
            };
        }

        [NotNull]
        public static PollInfo FromValid(bool isValid)
            => new PollInfo {IsValid = isValid};
        
        public Dictionary<string, string> Fields { get; }
        public bool IsValid { get; set; }
        public string Footer { get; } = "Click one of the numbers above to vote. Note: you can only vote for one.";

        public PollInfo()
        {
            Fields = new Dictionary<string, string>();
            IsValid = true;
        }

        public PollInfo AddFields(params (string Name, string Value)[] fields)
        {
            foreach (var (name, value) in fields)
            {
                if (name.IsNullOrEmpty() || value.IsNullOrEmpty()) continue;
                Fields.Add(name, value);
            }

            IsValid = true;

            return this;
        }
    }
}
