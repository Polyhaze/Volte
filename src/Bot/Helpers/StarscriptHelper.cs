using Starscript;

namespace Volte.Helpers;

public static class StarscriptHelper
{
    public static ValueMap Wrap(SocketUserMessage message)
    {
        var mMap = new ValueMap();
        mMap.SetToString(message.ToString);
        mMap.Set("author", Wrap(message.Author));
        mMap.Set("content", message.Content);
        mMap.Set("id", message.Id.ToString());
        return mMap;
    }
    
    public static ValueMap Wrap(SocketUser user)
    {
        var uMap = new ValueMap();
        uMap.SetToString(user.ToString);
        uMap.Set("username", user.Username);
        uMap.Set("isBot", user.IsBot);
        uMap.Set("hasPrimaryGuild", user.PrimaryGuild.HasValue);
        
        if (user.PrimaryGuild.HasValue)
            uMap.Set("primaryGuild", Wrap(user.PrimaryGuild.Value));
        
        uMap.Set("id", user.Id);
        return uMap;
    }

    public static ValueMap Wrap(PrimaryGuild primaryGuild)
    {
        var gMap = new ValueMap();
        gMap.Set("guildId", primaryGuild.GuildId?.ToString()!);
        gMap.Set("tag", primaryGuild.Tag);
        gMap.Set("enabled", primaryGuild.IdentityEnabled!);
        return gMap;
    }
}