using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Volte.Entities;
using Volte.Helpers;
using Volte.Interactions;
using Volte.Services;

namespace Volte.Commands.Application
{
    public class CountMembersCommand : ApplicationCommand
    {
        public CountMembersCommand() : base("count-members",
            "Counts the amount of members in the current guild have the provided role.", true) =>
            Signature(o =>
                o.RequiredRole("role", "The role to count members of."));

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var role = ctx.Options["role"].GetAsRole();

            var users = (await ctx.Guild.GetUsersAsync().FlattenAsync())
                .Where(x => x.RoleIds.Contains(role.Id))
                .ToArray();

            var result = new StringBuilder().Apply(sb =>
            {
                sb.Append(
                    $"There {"is".ToQuantity(users.Length).Split(" ")[1]} {"member".ToQuantity(users.Length)} in the role {role.Mention}");
                sb.Append(users.Any(x => x.Id == ctx.User.Id)
                    ? "; including you."
                    : ".");
            }).ToString();

            await ctx.CreateReplyBuilder()
                .WithEmbedFrom(result)
                .WithEphemeral()
                .RespondAsync();
        }
    }

    public sealed class UptimeCommand : ApplicationCommand
    {
        public UptimeCommand() : base("uptime", "Shows the bot's uptime.") { }

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
            => await ctx.CreateReplyBuilder(true)
                .WithEmbed(e => e.WithTitle(Process.GetCurrentProcess().CalculateUptime()))
                .RespondAsync();
    }

    public class AvatarCommand : ApplicationCommand
    {
        public AvatarCommand() : base("avatar", "Gets the avatar of the desired user or yourself.") =>
            Signature(o =>
                o.OptionalUser("user", "The user's avatar you want to see."));

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var user = ctx.Options["user"]?.GetAsGuildUser() ?? ctx.GuildUser;
            var buttons = new ushort[] { 128, 256, 512, 1024 }
                .Select(x => ButtonBuilder.CreateLinkButton($"{x}x{x}", user.GetEffectiveAvatarUrl(size: x)))
                .ToArray();

            await ctx.CreateReplyBuilder()
                .WithEmbeds(ctx.CreateEmbedBuilder()
                    .WithAuthor(user)
                    .WithImageUrl(user.GetEffectiveAvatarUrl()))
                .WithEphemeral()
                .WithButtons(buttons.Take(2))
                .WithButtons(buttons.Skip(2))
                .RespondAsync();
        }
    }


    public sealed class PingCommand : ApplicationCommand
    {
        public PingCommand() : base("ping", "Show the Gateway and REST latency from my servers to Discord.") { }

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var sw = Stopwatch.StartNew();
            await ctx.Interaction.DeferAsync(true)
                .Then(async () =>
                {
                    sw.Stop();
                    await ctx.CreateReplyBuilder()
                        .WithEmbedFrom(new StringBuilder()
                            .AppendLine($"{DiscordHelper.Clap} **Gateway**: {ctx.Client.Latency} milliseconds")
                            .AppendLine($"{DiscordHelper.OkHand} **REST**: {sw.Elapsed.Humanize(3)}"))
                        .WithEphemeral()
                        .FollowupAsync();
                });
        }
    }

    public class PollCommand : ApplicationCommand
    {
        public PollCommand() : base("poll", "Create a poll.") => Signature(o =>
        {
            o.RequiredString("question", "What would you like to ask?");
            o.RequiredString("options", "The options you want on the poll, separated by a semicolon (;). Limit 9.");
            o.OptionalMentionable("mention", "The user or role to notify about the newly created poll.");
        });

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var question = ctx.Options["question"].GetAsString();
            var options = ctx.Options["options"].GetAsString();
            var mention = ctx.Options["mention"];
            var mentionStr = mention != null
                ? mention.GetAsRole()?.Mention ?? mention.GetAsUser()?.Mention
                : string.Empty;

            if (PollInfo.TryParse($"{question};{options}", out var pollInfo))
            {
                await ctx.DeferAsync(true);
                Executor.Execute(async () =>
                {
                    await ctx.CreateReplyBuilder(true)
                        .WithEmbeds(ctx.CreateEmbedBuilder().Apply(pollInfo.Apply))
                        .FollowupAsync().Then(async m =>
                        {
                            if (mentionStr != null)
                                await ctx.Channel.SendMessageAsync(mentionStr);

                            await DiscordHelper.GetPollEmojis()[..pollInfo.Fields.Count]
                                .ForEachAsync(async emoji => await m.AddReactionAsync(emoji));
                        });
                });
            }
            else
            {
                await ctx.CreateReplyBuilder()
                    .WithEmbedFrom(pollInfo.Validation.InvalidationReason)
                    .WithEphemeral()
                    .RespondAsync();
            }
        }
    }

    public sealed class SpotifyCommand : ApplicationCommand
    {
        public SpotifyCommand() : base("spotify",
            "Shows what you or someone else is listening to on Spotify, if they are.") => Signature(o =>
            o.OptionalUser("user", "The member whose Spotify status you want to see. Defaults to yourself."));

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var target = ctx.Options["user"].GetAsGuildUser() ?? ctx.GuildUser;

            if (!target.TryGetSpotifyStatus(out var spotify))
                await ctx.CreateReplyBuilder()
                    .WithEmbedFrom("Target user isn't listening to Spotify!")
                    .WithEphemeral()
                    .RespondAsync();
            else
            {
                await ctx.CreateReplyBuilder()
                    .WithEmbeds(ctx.CreateEmbedBuilder()
                        .WithAuthor(target)
                        .AppendDescriptionLine($"**Track:** {Format.Url(spotify.TrackTitle, spotify.TrackUrl)}")
                        .AppendDescriptionLine($"**Album:** {spotify.AlbumTitle}")
                        .AppendDescriptionLine(
                            $"**Duration:** {(spotify.Duration.HasValue ? spotify.Duration.Value.Humanize(2) : "<not provided>")}")
                        .AppendDescriptionLine($"**Artist(s):** {spotify.Artists.Join(", ")}")
                        .AppendDescriptionLine(
                            $"**Started At:** {spotify.StartedAt?.GetDiscordTimestamp(TimestampType.LongTime) ?? "<not provided>"}")
                        .AppendDescriptionLine(
                            $"**Ends At:** {spotify.EndsAt?.GetDiscordTimestamp(TimestampType.LongTime) ?? "<not provided>"}")
                        .WithThumbnailUrl(spotify.AlbumArtUrl))
                    .WithEphemeral(target.Id != ctx.User.Id)
                    .RespondAsync();
            }
        }
    }

    public sealed class BigEmojiCommand : ApplicationCommand
    {
        private EmbedBuilder GenerateEmbed(IEmote emoteIn, MessageCommandContext ctx)
            => emoteIn switch
            {
                Emote emote => ctx.CreateEmbedBuilder(Format.Url("Direct Link", emote.Url))
                    .AddField("Created", emote.CreatedAt.GetDiscordTimestamp(TimestampType.Relative), true)
                    .AddField("Animated?", emote.Animated ? "Yes" : "No", true)
                    .WithImageUrl(emote.Url)
                    .WithAuthor($":{emote.Name}:", emote.Url),
                Emoji emoji => ctx.CreateEmbedBuilder(Format.Url("Direct Link", emoji.GetUrl()))
                    .AddField("Raw", Format.Code(emoji.Name))
                    .WithImageUrl(emoji.GetUrl()),
                _ => throw new ArgumentException("GenerateEmbed's parameter must be an Emote or an Emoji.")
            };

        public BigEmojiCommand() : base("Show All Emojis", ApplicationCommandType.Message) { }

        public override async Task HandleMessageCommandAsync(MessageCommandContext ctx)
        {
            var reply = ctx.CreateReplyBuilder(true);
            var parser = ctx.Services.Get<CommandsService>().GetTypeParser<IEmote>();
            var emojis = ctx.UserMessage.Content.Split(' ')
                .Select(x => parser.ParseAsync(x).Result) //it's non-async parsing logic, whatever
                .Where(res => res.IsSuccessful)
                .Select(res => res.Value)
                .ToList();
            if (emojis.IsEmpty())
                reply.WithEmbedFrom("Message contained no recognized emojis or accessible emotes.");
            else
                reply.WithEmbeds(emojis.Select(x => GenerateEmbed(x, ctx)));

            await reply.RespondAsync();
        }
    }

    public sealed class SnowflakeCommand : ApplicationCommand
    {
        public SnowflakeCommand() : base("snowflake",
            "Shows when the object with the given Snowflake ID was created.") => Signature(o =>
            o.RequiredString("snowflake", "The Discord snowflake you want to see."));

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            await ctx.CreateReplyBuilder(true)
                .WithEmbeds(ctx.CreateEmbedBuilder()
                    .WithTitle(SnowflakeUtils.FromSnowflake(ctx.Options["snowflake"].GetAsLong())
                        .GetDiscordTimestamp(TimestampType.LongDateTime)))
                .RespondAsync();
        }
    }

    public sealed class InfoCommand : ApplicationCommand
    {
        public sealed class UserContextCommand : ApplicationCommand
        {
            public UserContextCommand() : base("User Information", ApplicationCommandType.User) { }

            public override async Task HandleUserCommandAsync(UserCommandContext ctx)
            {
                await ctx.CreateReplyBuilder(true)
                    .WithEmbeds(ctx.CreateEmbedBuilder()
                        .WithTitle(ctx.TargetedGuildUser.ToString())
                        .AddField("ID", ctx.TargetedGuildUser.Id, true)
                        .AddField("Activity", GetRelevantActivity(ctx.TargetedGuildUser), true)
                        .AddField("Status", ctx.TargetedGuildUser.Status, true)
                        .AddField("Is Bot", ctx.TargetedGuildUser.IsBot ? "Yes" : "No", true)
                        .AddField("Role Hierarchy", ctx.TargetedGuildUser.Hierarchy, true)
                        .AddField("Account Created",
                            $"{ctx.TargetedGuildUser.CreatedAt.GetDiscordTimestamp(TimestampType.LongDateTime)}")
                        .AddField("Joined This Guild",
                            $"{(ctx.TargetedGuildUser.JoinedAt.HasValue ? ctx.TargetedGuildUser.JoinedAt.Value.GetDiscordTimestamp(TimestampType.LongDateTime) : DiscordHelper.Zws)}")
                        .WithThumbnailUrl(ctx.TargetedGuildUser.GetEffectiveAvatarUrl(size: 512)))
                    .RespondAsync();
            }
        }

        public InfoCommand() : base("info", "Gets information for Discord things.") => Signature(o =>
        {
            o.Subcommand("guild", "Show information about the current guild.");
            o.Subcommand("invite", "Get useful links for Volte such as the GitHub and Invite URL.");
            o.Subcommand("bot", "Show information about the current instance of Volte.");
            o.Subcommand("user", "Show information about a user, or yourself.", x =>
                x.OptionalUser("target", "The user to show information for")
            );
        });

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var subcommand = ctx.Options.First().Value;
            var reply = ctx.CreateReplyBuilder().WithEphemeral();
            if (subcommand.Name is "invite")
            {
                reply.WithButtons(
                    ButtonBuilder.CreateLinkButton("GitHub", "https://github.com/Ultz/Volte"),
                    ButtonBuilder.CreateLinkButton("Invite Me", ctx.Client.GetInviteUrl()),
                    ButtonBuilder.CreateLinkButton("Server", "https://discord.gg/H8bcFr2")
                );
            }

            switch (subcommand.Name)
            {
                case "guild":
                    reply.WithEmbeds(ctx.CreateEmbedBuilder()
                        .WithTitle(ctx.Guild.Name)
                        .AddField("Created", $"{ctx.Guild.CreatedAt.GetDiscordTimestamp(TimestampType.LongDateTime)}")
                        .AddField("Owner", ctx.Guild.Owner)
                        .AddField("Region", ctx.Guild.VoiceRegionId)
                        .AddField("Members", ctx.Guild.Users.Count, true)
                        .AddField("Roles", ctx.Guild.Roles.Count, true)
                        .AddField("Category Channels", ctx.Guild.CategoryChannels.Count, true)
                        .AddField("Voice Channels", ctx.Guild.VoiceChannels.Count, true)
                        .AddField("Text Channels", ctx.Guild.TextChannels.Count, true)
                        .WithThumbnailUrl(ctx.Guild.IconUrl));
                    break;
                case "invite":
                    reply.WithEmbedFrom(new StringBuilder()
                        .AppendLine(
                            $"Do you like Volte? If you do, that's awesome! If not then I'm sorry (please tell me what you don't like {Format.Url("here", "https://forms.gle/CJ9XtKmKf2Q2mQwb7")}!) :( ")
                        .AppendLine()
                        .AppendLine("Thanks for having me in your server!")
                        .ToString());
                    break;
                case "bot":
                    reply.WithEmbeds(ctx.CreateEmbedBuilder()
                        .AddField("Version", Version.FullVersion, true)
                        .AddField("Author",
                            $"{await ctx.Client.Rest.GetUserAsync(168548441939509248)}, contributors on {Format.Url("GitHub", "https://github.com/Ultz/Volte")}, and members of the Ultz organization.",
                            true)
                        .AddField("Language/Library", $"C# 8, Discord.Net {Version.DiscordNetVersion}", true)
                        .AddField("Discord Application Created",
                            (await ctx.Client.GetApplicationInfoAsync()).CreatedAt.GetDiscordTimestamp(TimestampType
                                .LongDateTime))
                        .AddField("Guilds", ctx.Client.Guilds.Count, true)
                        .AddField("Shards", ctx.Client.Shards.Count, true)
                        .AddField("Channels",
                            ctx.Client.Guilds.SelectMany(x => x.Channels).Where(x => !(x is SocketCategoryChannel))
                                .DistinctBy(x => x.Id).Count(),
                            true)
                        .AddField("Uptime", Process.GetCurrentProcess().CalculateUptime(), true)
                        .WithThumbnailUrl(ctx.Client.CurrentUser.GetEffectiveAvatarUrl(size: 512)));
                    break;
                case "user":
                    var target = subcommand.GetOption("target").GetAsGuildUser(ctx.GuildUser);

                    reply.WithEmbeds(ctx.CreateEmbedBuilder()
                        .WithTitle(target.ToString())
                        .AddField("ID", target.Id, true)
                        .AddField("Activity", GetRelevantActivity(target), true)
                        .AddField("Status", target.Status, true)
                        .AddField("Is Bot", target.IsBot ? "Yes" : "No", true)
                        .AddField("Role Hierarchy", target.Hierarchy, true)
                        .AddField(
                            "Account Created",
                            $"{target.CreatedAt.GetDiscordTimestamp(TimestampType.LongDateTime)}"
                        ).Apply(eb =>
                        {
                            if (target.JoinedAt.HasValue)
                                eb.AddField("Joined This Guild",
                                    target.JoinedAt.Value.GetDiscordTimestamp(TimestampType.LongDateTime));
                        })
                        .WithThumbnailUrl(target.GetEffectiveAvatarUrl(size: 512)));
                    break;
            }

            await reply.RespondAsync();
        }

        private static string GetRelevantActivity(IPresence member) => member.Activities.FirstOrDefault() switch
        {
            //we are ignoring custom emojis because there is no guarantee that volte is in the guild where the emoji is from; which could lead to a massive (and ugly) embed field value
            CustomStatusGame { Emote: Emoji _ } csg => $"{csg.Emote} {csg.State}",
            CustomStatusGame csg => $"{csg.State}",
            SpotifyGame _ => "Listening to Spotify",
            _ => member.Activities.FirstOrDefault()?.Name
        } ?? "Nothing";
    }
}