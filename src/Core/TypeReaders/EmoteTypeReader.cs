using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using System.Text.RegularExpressions;

namespace BrackeysBot
{
    public class EmoteTypeReader : TypeReader
    {
        private static readonly Regex _emotePattern = new Regex(@"^<a?:.+:\d+>$");
        
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            IEmote result;

            if (_emotePattern.Match(input).Success)
                result = Emote.Parse(input);
            else if (input.Length == 1)
                result = new Emoji(input);
            else 
                throw new ArgumentException($"Value {input} is not a valid emote");
                
            return TypeReaderResult.FromSuccess(result);
        }

    }
}