using System.Collections.Generic;
using System.Linq;
using Gommon;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Volte.Services;

// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Core.Models.Misc
{
    public sealed class PollInfo
    {
        public static PollInfo FromFields(params (string Name, string Value)[] fields) 
            => new PollInfo().AddFields(fields);

        public static PollInfo FromDefaultFields(int count, EmojiService e, IEnumerable<string> choices)
        {
            var collection = choices as string[] ?? choices.ToArray();
            return count switch
            {
                1 => FromFields(($"{e.One.ToEmoji()}", collection[1])),

                2 => FromFields(($"{e.One.ToEmoji()}", collection[1]),
                    ($"{e.Two.ToEmoji()}", collection[2])),

                3 => FromFields(($"{e.One.ToEmoji()}", collection[1]),
                    ($"{e.Two.ToEmoji()}", collection[2]),
                    ($"{e.Three.ToEmoji()}", collection[3])),

                4 => FromFields(($"{e.One.ToEmoji()}", collection[1]),
                    ($"{e.Two.ToEmoji()}", collection[2]),
                    ($"{e.Three.ToEmoji()}", collection[3]),
                    ($"{e.Four.ToEmoji()}", collection[4])),

                5 => FromFields(($"{e.One.ToEmoji()}", collection[1]),
                    ($"{e.Two.ToEmoji()}", collection[2]),
                    ($"{e.Three.ToEmoji()}", collection[3]),
                    ($"{e.Four.ToEmoji()}", collection[4]),
                    ($"{e.Five.ToEmoji()}", collection[5])),

                _ => FromValid(false)
                
            };
        }

        public static PollInfo FromValid(bool isValid)
            => new PollInfo {IsValid = isValid};

        public Dictionary<string, string> Fields { get; }
        public bool IsValid { get; set; }
        public string Footer { get; } = "Click one of the numbers above to vote. Note: you can vote for more than one.";

        public PollInfo()
        {
            Fields = new Dictionary<string, string>();
            IsValid = true;
        }

        public PollInfo AddFields(params (string Name, string Value)[] fields)
        {
            foreach (var field in fields)
            {
                if (field.Name.IsNullOrEmpty() || field.Value.IsNullOrEmpty()) continue;
                Fields.Add(field.Name, field.Value);
            }

            return this;
        }
    }
}
