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
        uMap.Set("isWebhook", user.IsWebhook);
        uMap.Set("mention", user.Mention);
        uMap.Set("createdAt", user.CreatedAt.ToString($"{StandardLibrary.TimeFormat}, {StandardLibrary.DateFormat}"));
        uMap.Set("avatarUrl", user.GetDisplayAvatarUrl(size: 256));
        uMap.Set("hasPrimaryGuild", user.PrimaryGuild.HasValue);
        uMap.TryAddNullable("primaryGuild", user.PrimaryGuild, Wrap);
        
        uMap.Set("id", user.Id);
        return uMap;
    }
    
    public static ValueMap Wrap(SocketGuildUser user)
    {
        var uMap = Wrap((SocketUser)user);

        uMap.Set("nickname", user.Nickname);
        uMap.Set("isSuppressed", user.IsSuppressed);
        uMap.Set("isDeafened", user.IsDeafened);
        uMap.Set("isMuted", user.IsMuted);
        uMap.Set("isSelfDeafened", user.IsSelfDeafened);
        uMap.Set("isSelfMuted", user.IsSelfMuted);
        uMap.Set("isStreaming", user.IsStreaming);
        uMap.Set("isVideoing", user.IsVideoing);
        uMap.Set("hierarchy", user.Hierarchy);
        uMap.Set("highestRoleId", () => user.GetHighestRole()?.Id!);
        uMap.Set("highestRoleName", () => user.GetHighestRole()?.Name!);
        uMap.Set("isTimedOut", user.TimedOutUntil.HasValue);
        uMap.TryAddNullable("timedOutUntil", user.TimedOutUntil, v => v.ToString($"{StandardLibrary.TimeFormat}, {StandardLibrary.DateFormat}"));
        uMap.TryAddNullable("joinedAt", user.JoinedAt, v => v.ToString($"{StandardLibrary.TimeFormat}, {StandardLibrary.DateFormat}"));
        
        uMap.Set("hasRole", ctx =>
        {
            ctx.Constrain(Constraint.ExactlyOneArgument);

            if (!ulong.TryParse(ctx.NextString(1), out var id))
                throw ctx.Error($"The argument to {ctx.FormattedName} must be a Discord Snowflake ID in a string.");

            return user.HasRole(id);
        });

        return uMap;
    }

    private static void TryAddNullable<T>(this ValueMap map, string name, T? nullable, Func<T, Value> valueMapper)
        where T : struct
    {
        if (nullable.HasValue)
            map.Set(name, valueMapper(nullable.Value));
    }
    
    private static void TryAddNullable<T>(this ValueMap map, string name, T? nullable, Func<T, ValueMap> valueMapper)
        where T : struct
    {
        if (nullable.HasValue)
            map.Set(name, valueMapper(nullable.Value));
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