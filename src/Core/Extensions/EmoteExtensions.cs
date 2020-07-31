using Discord;

namespace BrackeysBot
{
    public static class EmoteExtensions
    {
        public static string StringVal(this IEmote iEmote) 
            => iEmote is Emote emote ? $":{emote.Name}:{emote.Id}" : iEmote.Name;
        
        public static string Emotify(this IEmote iEmote) 
            => iEmote is Emote emote ? $"<:{emote.Name}:{emote.Id}>" : iEmote.Name;
        
    }
}
