using System.Collections.Generic;
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

        public static PollInfo FromDefaultFields(int count, EmojiService e, string[] choices)
        {
            return count switch
            {
                1 => FromFields(($"{e.One.ToEmoji()}", choices[1])),

                2 => FromFields(($"{e.One.ToEmoji()}", choices[1]),
                    ($"{e.Two.ToEmoji()}", choices[2])),

                3 => FromFields(($"{e.One.ToEmoji()}", choices[1]),
                    ($"{e.Two.ToEmoji()}", choices[2]),
                    ($"{e.Three.ToEmoji()}", choices[3])),

                4 => FromFields(($"{e.One.ToEmoji()}", choices[1]),
                    ($"{e.Two.ToEmoji()}", choices[2]),
                    ($"{e.Three.ToEmoji()}", choices[3]),
                    ($"{e.Four.ToEmoji()}", choices[4])),

                5 => FromFields(($"{e.One.ToEmoji()}", choices[1]),
                    ($"{e.Two.ToEmoji()}", choices[2]),
                    ($"{e.Three.ToEmoji()}", choices[3]),
                    ($"{e.Four.ToEmoji()}", choices[4]),
                    ($"{e.Five.ToEmoji()}", choices[5])),

                _ => FromValid(false)
                
            };
        }

        public static PollInfo FromValid(bool isValid)
            => new PollInfo {IsValid = isValid};

        public List<(string Name, string Value)> Fields { get; }
        public bool IsValid { get; set; }
        public string Footer { get; } = "Click the number below to vote.";

        public PollInfo()
        {
            Fields = new List<(string Name, string Value)>();
            IsValid = true;
        }

        public PollInfo AddFields(params (string Name, string Value)[] fields)
        {
            foreach (var field in fields)
            {
                if (field.Name.IsNullOrEmpty() || field.Value.IsNullOrEmpty()) continue;
                Fields.Add(field);
            }

            return this;
        }
    }
}
