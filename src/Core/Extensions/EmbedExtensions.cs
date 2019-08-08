using System.Threading.Tasks;
using Discord;
 
using Volte.Core;

namespace Gommon
{
    public static partial class Extensions
    {
        public static Task<IUserMessage> SendToAsync(this EmbedBuilder e, IMessageChannel c)
        {
            return c.SendMessageAsync(string.Empty, false, e.Build());
        }

        public static Task<IUserMessage> SendToAsync(this Embed e, IMessageChannel c)
        {
            return c.SendMessageAsync(string.Empty, false, e);
        }

        public static async Task<IUserMessage> SendToAsync(this EmbedBuilder e, IGuildUser u)
        {
            return await (await u.GetOrCreateDMChannelAsync()).SendMessageAsync(string.Empty, false, e.Build());
        }

        public static async Task<IUserMessage> SendToAsync(this Embed e, IGuildUser u)
        {
            return await (await u.GetOrCreateDMChannelAsync()).SendMessageAsync(string.Empty, false, e);
        }

        public static EmbedBuilder WithSuccessColor(this EmbedBuilder e)
        {
            return e.WithColor(Config.SuccessColor);
        }

        public static EmbedBuilder WithErrorColor(this EmbedBuilder e)
        {
            return e.WithColor(Config.ErrorColor);
        }
    }
}