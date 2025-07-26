using Starscript;

namespace Volte.Helpers;

public static class StarscriptHelper
{
    public static ValueMap Wrap(IGuild guild, IUser user) =>
        new ValueMap()
            .Set("guild", Wrap(guild))
            .Set("user", Wrap(user));

    public static async Task<ValueMap> WrapAsync(IGuild guild, IUser user)
    {
        var map = new ValueMap();
        map.Set("user", Wrap(user));
        map.Set("guild", await WrapAsync(guild));
        return map;
    }

    public static ValueMap Wrap(SocketUserMessage message)
    {
        var mMap = new ValueMap();
        mMap.SetToString(message.ToString);
        mMap.Set("author", Wrap(message.Author));
        mMap.Set("content", message.Content);
        mMap.Set("id", message.Id.ToString());
        return mMap;
    }

    private static ValueMap WrapBase(IGuild guild)
    {
        var gMap = new ValueMap();
        gMap.SetToString(guild.ToString);
        gMap.Set("name", guild.Name);
        gMap.Set("id", guild.Id);
        gMap.Set("mfaLevel", Enum.GetName(guild.MfaLevel)!);
        gMap.Set("verificationLevel", Enum.GetName(guild.VerificationLevel)!);
        gMap.Set("explicitContentFilter", Enum.GetName(guild.ExplicitContentFilter)!);
        gMap.Set("nsfwLevel", Enum.GetName(guild.NsfwLevel)!);
        gMap.Set("description", guild.Description);
        gMap.Set("hasVanityInvite", guild.VanityURLCode != null);
        if (guild.VanityURLCode != null)
            gMap.Set("vanityInvite", $"https://discord.gg/{guild.VanityURLCode}");
        gMap.Set("boostTier", Enum.GetName(guild.PremiumTier)!);
        gMap.Set("boostCount", guild.PremiumSubscriptionCount);
        gMap.TryAddNullable("maxPresences", guild.MaxPresences);
        gMap.TryAddNullable("maxMembers", guild.MaxMembers);
        gMap.TryAddNullable("maxVideoChannelUsers", guild.MaxVideoChannelUsers);
        gMap.TryAddNullable("maxStageVideoChannelUsers", guild.MaxStageVideoChannelUsers);
        gMap.TryAddNullable("approximateMemberCount", guild.ApproximateMemberCount);
        gMap.TryAddNullable("approximatePresenceCount", guild.ApproximatePresenceCount);
        gMap.TryAddNullable("afkChannelId", guild.AFKChannelId);
        gMap.TryAddNullable("widgetChannelId", guild.WidgetChannelId);
        gMap.TryAddNullable("safetyAlertsChannelId", guild.SafetyAlertsChannelId);
        gMap.TryAddNullable("systemChannelId", guild.SystemChannelId);
        gMap.TryAddNullable("rulesChannelId", guild.RulesChannelId);
        gMap.TryAddNullable("pubicUpdatesChannelId", guild.PublicUpdatesChannelId);
        gMap.Set("maxBitrate", guild.MaxBitrate);
        gMap.Set("preferredLocale", guild.PreferredLocale);
        gMap.Set("isBoostProgressBarEnabled", guild.IsBoostProgressBarEnabled);
        gMap.Set("voiceRegionId", guild.VoiceRegionId);
        return gMap;
    }

    public static ValueMap Wrap(IGuild guild) 
        => WrapBase(guild)
            .Set("ownerId", guild.OwnerId);

    public static async ValueTask<ValueMap> WrapAsync(IGuild guild) =>
        WrapBase(guild)
            .Set("owner",
                Wrap(
                    guild is SocketGuild sGuild 
                        ? sGuild.Owner 
                        : await guild.GetOwnerAsync()));


    public static ValueMap Wrap(IUser user)
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

    public static ValueMap Wrap(IGuildUser user)
    {
        var uMap = Wrap((IUser)user);

        uMap.Set("nickname", user.Nickname);
        uMap.Set("isSuppressed", user.IsSuppressed);
        uMap.Set("isDeafened", user.IsDeafened);
        uMap.Set("isMuted", user.IsMuted);
        uMap.Set("isSelfDeafened", user.IsSelfDeafened);
        uMap.Set("isSelfMuted", user.IsSelfMuted);
        uMap.Set("isStreaming", user.IsStreaming);
        uMap.Set("isVideoing", user.IsVideoing);
        uMap.Set("hierarchy", user.Hierarchy);
        uMap.Set("isTimedOut", user.TimedOutUntil.HasValue);
        uMap.TryAddNullable("timedOutUntil", user.TimedOutUntil,
            v => v.ToString($"{StandardLibrary.TimeFormat}, {StandardLibrary.DateFormat}"));
        uMap.TryAddNullable("joinedAt", user.JoinedAt,
            v => v.ToString($"{StandardLibrary.TimeFormat}, {StandardLibrary.DateFormat}"));

        uMap.Set("hasRole", ctx =>
        {
            ctx.Constrain(Constraint.ExactlyOneArgument);

            if (!ulong.TryParse(ctx.NextString(1), out var id))
                throw ctx.Error($"The argument to {ctx.FormattedName} must be a Discord Snowflake ID in a string.");

            return user.RoleIds.Contains(id);
        });

        return uMap;
    }

    private static void TryAddNullable<T>(this ValueMap self, string name, T? nullable,
        Func<T, Value> valueMapper = null)
        where T : struct
    {
        if (nullable.HasValue)
            self.Set(name, valueMapper?.Invoke(nullable.Value) ?? nullable.Value.ToString()!);
        else
            self.Set(name, Value.Null);
    }

    private static void TryAddNullable<T>(this ValueMap self, string name, T? nullable, Func<T, ValueMap> valueMapper)
        where T : struct
    {
        if (nullable.HasValue)
            self.Set(name,
                valueMapper != null
                    ? valueMapper(nullable.Value)
                    : new ValueMap().SetToString(() => nullable.Value.ToString()!));
        else
            self.Set(name, Value.Null);
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