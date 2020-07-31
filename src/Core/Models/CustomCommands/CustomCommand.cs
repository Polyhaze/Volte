using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;

namespace BrackeysBot.Core.Models
{
    public class CustomCommand
    {
        public string Name { get; set; }
        public List<CustomCommandFeature> Features { get; set; }
        public bool IgnoreArguments { get; set; }

        private CustomCommand() { }
        public CustomCommand(string name)
        {
            Name = name;
            Features = new List<CustomCommandFeature>();
        }

        public bool Matches(string name, string[] args)
        {
            bool argsMatch = MatchesArgs(args);
            if (Features.Find(c => c is AliasFeature) is AliasFeature alias 
                && alias.Matches(name)
                && argsMatch) 
                return true;

            return Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && argsMatch;
        }

        public virtual async Task ExecuteCommand(ICommandContext context)
        {
            if (Features.Count > 0)
            {
                foreach (CustomCommandFeature feature in Features)
                {
                    await feature.Execute(context);
                }
            }
            else
            {
                await context.Channel.SendMessageAsync(string.Empty, false, new EmbedBuilder().WithDescription("No features have been set yet!").Build());
            }
        }

        private bool MatchesArgs(string[] args) 
        {
            return IgnoreArguments || true; // TODO add arguments and arg checks. Because arguments don't do anything yet we can just return true for now.
        }
    }
}
