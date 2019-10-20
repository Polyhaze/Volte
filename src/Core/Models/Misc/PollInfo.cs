using System.Collections.Generic;
using Gommon;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Volte.Core.Models.Misc
{
    public sealed class PollInfo
    {
        public static PollInfo FromFields(params (string Name, string Value)[] fields) 
            => new PollInfo().AddFields(fields);

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
