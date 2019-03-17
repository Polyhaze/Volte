using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.TypeParsers
{
    public sealed class EmoteParser<TEmoji> : TypeParser<TEmoji> where TEmoji : DiscordEmoji
    {
        public override Task<TypeParserResult<TEmoji>> ParseAsync(
            Parameter param,
            string value, 
            ICommandContext context, 
            IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();



            try
            {
                return Task.FromResult(
                    new TypeParserResult<TEmoji>(DiscordEmoji.FromName(ctx.Client, value).Cast<TEmoji>()));
            }
            catch (ArgumentException)
            {
                return Task.FromResult(TypeParserResult<TEmoji>.Unsuccessful("Specified emoji/emote not found."));
            }
        }
    }
}
