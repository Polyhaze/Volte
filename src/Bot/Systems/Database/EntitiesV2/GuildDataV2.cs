using Volte.Systems.UserFilter;

namespace Volte.Systems.Database.EntitiesV2;

public class GuildDataV2
{
    public ulong Id { get; set; }

    public ulong OwnerId { get; set; }

    public StarscriptTables StarscriptTables { get; set; } = new();

    public ModerationData Moderation { get; set; } = new();

    public GuildSettings Settings { get; set; } = new();
    
    public void AddTag(TagV2 tag)
    {
        var existingIdenticalTag = Settings.Collections.Tags.FirstOrDefault(it => it.Response == tag.Response);
        if (existingIdenticalTag is not null)
        {
            Settings.Collections.Tags.Remove(existingIdenticalTag);
            existingIdenticalTag.Aliases.Add(tag.Name);
            Settings.Collections.Tags.Add(existingIdenticalTag);
        }
        else
        {
            Settings.Collections.Tags.Add(tag);
        }
    }

    public Gommon.Optional<TagV2> GetTagByNameOrAlias(string nameOrAlias)
        => Settings.Collections.Tags.FindFirst(tag => tag.Name.EqualsIgnoreCase(nameOrAlias)
                                 || nameOrAlias.EqualsAnyIgnoreCase(tag.Aliases.ToArray())
        );

    public static GuildDataV2 CreateFrom(IGuild guild)
        => new()
        {
            Id = guild.Id,
            OwnerId = guild.OwnerId,
            Settings = new GuildSettings
            {
                Autorole = default,
                CommandPrefix = Config.CommandPrefix,
                ReplyInline = false,
                EmbedTags = false,
                AutoQuoteMessageUrls = false,
                Moderation = new ModerationSettings
                {
                    ActionLogChannel = default,
                    AdminRole = default,
                    ModRole = default,
                    SecondaryModRole = default,
                    CheckAccountAge = false,
                    VerifiedRole = default,
                    UnverifiedRole = default,
                    ShowResponsibleModerator = true
                },
                Welcome = new WelcomeSettings
                {
                    JoinMessage = string.Empty,
                    JoinDmMessage = string.Empty,
                    LeftMessage = string.Empty,
                    Channel = default,
                    EmbedColor = new Color(0x7000FB).RawValue
                },
                Starboard = new StarboardSettings
                {
                    Channel = default,
                    DeleteInvalidStars = true,
                    Enabled = false,
                    StarsRequiredToPost = 1
                },
                Collections = new GuildSettings.SettingsCollections
                {
                    SelfRoles = [],
                    Tags = []
                }
            },
            Moderation = new ModerationData
            {
                CurrentModActionCase = 0,
                Warns = []
            },
            StarscriptTables = new StarscriptTables
            {
                UserFilter = new UserFilterTable()
            }
        };

    public static GuildDataV2 MigrateFromV1(Entities.GuildData v1) => new()
    {
        Id = v1.Id,
        OwnerId = v1.OwnerId,
        Settings = new GuildSettings
        {
            Autorole = v1.Configuration.Autorole,
            CommandPrefix = v1.Configuration.CommandPrefix,
            ReplyInline = v1.Configuration.ReplyInline,
            EmbedTags = v1.Configuration.EmbedTagsAndShowAuthor,
            AutoQuoteMessageUrls = v1.Extras.AutoParseQuoteUrls,
            Moderation = new ModerationSettings
            {
                ActionLogChannel = v1.Configuration.Moderation.ModActionLogChannel,
                AdminRole = v1.Configuration.Moderation.AdminRole,
                ModRole = v1.Configuration.Moderation.ModRole,
                SecondaryModRole = default,
                CheckAccountAge = v1.Configuration.Moderation.CheckAccountAge,
                VerifiedRole = v1.Configuration.Moderation.VerifiedRole,
                UnverifiedRole = v1.Configuration.Moderation.UnverifiedRole,
                ShowResponsibleModerator = v1.Configuration.Moderation.ShowResponsibleModerator
            },
            Welcome = new WelcomeSettings
            {
                JoinMessage = v1.Configuration.Welcome.WelcomeMessage,
                JoinDmMessage = v1.Configuration.Welcome.WelcomeDmMessage,
                LeftMessage = v1.Configuration.Welcome.LeavingMessage,
                Channel = v1.Configuration.Welcome.WelcomeChannel,
                EmbedColor = v1.Configuration.Welcome.WelcomeColor
            },
            Starboard = new StarboardSettings
            {
                Channel = v1.Configuration.Starboard.StarboardChannel,
                DeleteInvalidStars = v1.Configuration.Starboard.DeleteInvalidStars,
                Enabled = v1.Configuration.Starboard.Enabled,
                StarsRequiredToPost = v1.Configuration.Starboard.StarsRequiredToPost
            },
            Collections = new GuildSettings.SettingsCollections
            {
                SelfRoles = v1.Extras.SelfRoles,
                Tags = v1.Extras.Tags.Select(tagV1 => new TagV2
                {
                    Name = tagV1.Name,
                    CreatorId = tagV1.CreatorId,
                    GuildId = tagV1.GuildId,
                    Response = tagV1.Response,
                    Aliases = tagV1.Aliases,
                    Uses = (ulong)tagV1.Uses
                }).ToHashSet()
            }
        },
        Moderation = new ModerationData
        {
            CurrentModActionCase = v1.Extras.ModActionCaseNumber,
            Warns = v1.Extras.Warns.Select(warnV1 => new WarnV2
            {
                Issuer = warnV1.Issuer,
                Target = warnV1.User,
                Reason = warnV1.Reason,
                Date = warnV1.Date
            }).ToList()
        },
        StarscriptTables = new StarscriptTables
        {
            UserFilter = new UserFilterTable
            {
                Entries = v1.Extras.StarscriptTables.UserFilter.Entries
            }
        }
    };
}