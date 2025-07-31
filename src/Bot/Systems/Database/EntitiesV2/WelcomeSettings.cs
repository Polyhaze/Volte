using Volte.Systems.Starscript;

namespace Volte.Systems.Database.EntitiesV2;

public class WelcomeSettings
{
    public static readonly Dictionary<string, string> ValidPlaceholders = new()
    {
        {"GuildName", "The name of the guild."},
        {"UserName", "The user's name."},
        {"UserMention", "The user's full @."}, 
        {"OwnerMention", "The guild owner's full @."},
        {"UserTag", "The user's discriminator (the numbers after their #)."},
        {"MemberCount", "The amount of people in the guild."},
        {"UserString", "A user's full username#discriminator; i.e. Greem#1337."}
    };
    
    public ulong Channel { get; set; }
    
    public string JoinMessage { get; set; }
    
    public string JoinDmMessage { get; set; }
    
    public string LeftMessage { get; set; }
    
    public uint EmbedColor { get; set; }

    public async ValueTask<string> FormatJoinMessageAsync(SocketGuildUser user)
    {
        var msg = JoinMessage.ReplaceIgnoreCase("{ServerName}", user.Guild.Name)
            .ReplaceIgnoreCase("{GuildName}", user.Guild.Name)
            .ReplaceIgnoreCase("{UserName}", user.Username)
            .ReplaceIgnoreCase("{UserMention}", user.Mention)
            .ReplaceIgnoreCase("{OwnerMention}", user.Guild.Owner.Mention)
            .ReplaceIgnoreCase("{UserTag}", user.Discriminator)
            .ReplaceIgnoreCase("{MemberCount}", user.Guild.MemberCount)
            .ReplaceIgnoreCase("{UserString}", user);

        try
        {
            return VolteStarscript.Run(msg, await StarscriptHelper.WrapAsync(user.Guild, user)).ToString();
        }
        catch
        {
            return msg;
        }
    }

    public async ValueTask<string> FormatLeftMessageAsync(SocketGuild guild, SocketUser user)
    {
        var msg = LeftMessage.ReplaceIgnoreCase("{ServerName}", guild.Name)
            .ReplaceIgnoreCase("{GuildName}", guild.Name)
            .ReplaceIgnoreCase("{UserName}", user.Username)
            .ReplaceIgnoreCase("{UserMention}", user.Mention)
            .ReplaceIgnoreCase("{OwnerMention}", guild.Owner.Mention)
            .ReplaceIgnoreCase("{UserTag}", user.Discriminator)
            .ReplaceIgnoreCase("{MemberCount}", guild.MemberCount)
            .ReplaceIgnoreCase("{UserString}", user);
        
        try
        {
            return VolteStarscript.Run(msg, await StarscriptHelper.WrapAsync(guild, user)).ToString();
        }
        catch
        {
            return msg;
        }
    }

    public async ValueTask<string> FormatJoinDmMessageAsync(SocketGuildUser user)
    {
        var msg = JoinDmMessage.ReplaceIgnoreCase("{ServerName}", user.Guild.Name)
            .ReplaceIgnoreCase("{GuildName}", user.Guild.Name)
            .ReplaceIgnoreCase("{UserName}", user.Username)
            .ReplaceIgnoreCase("{UserMention}", user.Mention)
            .ReplaceIgnoreCase("{OwnerMention}", user.Guild.Owner.Mention)
            .ReplaceIgnoreCase("{UserTag}", user.Discriminator)
            .ReplaceIgnoreCase("{MemberCount}", user.Guild.MemberCount)
            .ReplaceIgnoreCase("{UserString}", user);
        
        try
        {
            return VolteStarscript.Run(msg, await StarscriptHelper.WrapAsync(user.Guild, user)).ToString();
        }
        catch
        {
            return msg;
        }
    }

    public override string ToString()
        => JsonSerializer.Serialize(this, Config.JsonOptions);
}