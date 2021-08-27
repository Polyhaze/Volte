using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Commands.Slash.Commands
{
    public sealed class IamCommand : SlashCommand
    {
        public IamCommand() : base("iam", "Give yourself Self Roles via dropdown menu.") { }

        public override async Task HandleAsync(SlashCommandContext ctx)
        {
            var roles = ctx.GuildSettings.Extras.SelfRoles
                .Select(x => ctx.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(x)))
                .Where(x => x != null)
                .Where(x => !ctx.GuildUser.HasRole(x.Id))
                .ToList();

            var reply = ctx.CreateReplyBuilder(true);


            if (roles.IsEmpty())
                reply.WithEmbedFrom("This guild has no more roles available for self-assigning.");
            else
                reply.WithEmbedFrom("What roles would you like?")
                    .WithSelectMenu(new SelectMenuBuilder()
                        .WithCustomId("iam:menu")
                        .WithMaxValues(roles.Count)
                        .AddOptions(roles.Select(r => new SelectMenuOptionBuilder()
                            .WithLabel(r.Name)
                            .WithValue(r.Id.ToString()))
                        )
                    );

            await reply.RespondAsync();
        }

        public override async Task HandleComponentAsync(MessageComponentContext ctx)
        {
            await ctx.DeferAsync(true);
            if (ctx.CustomId.Split(':')[1] != "menu") return;
            
            await ctx.GuildUser.AddRolesAsync(ctx.SelectedMenuOptions.Select(x => ctx.Guild.GetRole(ulong.Parse(x))));
        }
    }

    public sealed class IamNotCommand : SlashCommand
    {
        public IamNotCommand() : base("iamnot", "Take away Self Roles from yourself via dropdown menu.") { }

        public override async Task HandleAsync(SlashCommandContext ctx)
        {
            var roles = ctx.GuildSettings.Extras.SelfRoles
                .Select(x => ctx.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(x)))
                .Where(x => x != null)
                .Where(x => ctx.GuildUser.HasRole(x.Id))
                .ToList();
            
            var reply = ctx.CreateReplyBuilder(true);


            if (roles.IsEmpty())
                reply.WithEmbedFrom("You don't have any self roles.");
            else
                reply.WithEmbedFrom("What roles would you like taken away?")
                    .WithSelectMenu(new SelectMenuBuilder()
                        .WithCustomId("iamnot:menu")
                        .WithMaxValues(roles.Count)
                        .AddOptions(roles.Select(r => new SelectMenuOptionBuilder()
                            .WithLabel(r.Name)
                            .WithValue(r.Id.ToString()))
                        )
                    );

            await reply.RespondAsync();
        }

        public override async Task HandleComponentAsync(MessageComponentContext ctx)
        {
            await ctx.DeferAsync(true);
            if (ctx.CustomId.Split(':')[1] != "menu") return;
            
            await ctx.GuildUser.RemoveRolesAsync(ctx.SelectedMenuOptions.Select(x => ctx.Guild.GetRole(ulong.Parse(x))));
        }
    }

    public class CountMembersCommand : SlashCommand
    {
        public CountMembersCommand() : base("count-members",
            "Counts the amount of members in the current guild have the provided role.") { }

        public override SlashCommandBuilder GetCommandBuilder(IServiceProvider provider)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("role")
                    .WithDescription("The role to count members in.")
                    .WithType(ApplicationCommandOptionType.Role)
                    .WithRequired(true));

        public override async Task HandleAsync(SlashCommandContext ctx)
        {
            var role = ctx.ValuedOptions["role"].Cast<SocketRole>();

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

    public sealed class UptimeCommand : SlashCommand
    {
        public UptimeCommand() : base("uptime", "Shows the bot's uptime.") { }

        public override async Task HandleAsync(SlashCommandContext ctx)
            => await ctx.CreateReplyBuilder(true)
                .WithEmbed(e => e.WithTitle(Process.GetCurrentProcess().CalculateUptime()))
                .RespondAsync();
    }

    public class AvatarCommand : SlashCommand
    {
        public AvatarCommand() : base("avatar", "Gets the avatar of the desired user or yourself.") { }

        public override SlashCommandBuilder GetCommandBuilder(IServiceProvider _)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithType(ApplicationCommandOptionType.User)
                    .WithName("user")
                    .WithDescription("The user's avatar you want to see.")
                    .WithRequired(false));

        public override async Task HandleAsync(SlashCommandContext ctx)
        {
            var user = ctx.Options["user"].GetValueOr<SocketGuildUser>(ctx.User);

            IMessageComponent GetLinkButton(ushort size)
                => ButtonBuilder.CreateLinkButton($"{size}x{size}", user.GetEffectiveAvatarUrl(size: size)).Build();

            string FormatEmbedString(params ushort[] sizes) => new StringBuilder().Apply(sb =>
            {
                sb.Append(sizes.Take(1)
                    .Select(x => $"{Format.Url(x.ToString(), user.GetEffectiveAvatarUrl(size: x))} ").First());
                sb.Append(sizes.Skip(1)
                    .Select(x => $"| {Format.Url(x.ToString(), user.GetEffectiveAvatarUrl(size: x))} ")
                    .Join(string.Empty));
            }).ToString();

            var buttons = new ushort[] { 128, 256, 512, 1024 }
                .Select(GetLinkButton)
                .ToArray();
            
            await ctx.CreateReplyBuilder()
                .WithEmbeds(ctx.CreateEmbedBuilder()
                    .WithAuthor(user)
                    .WithImageUrl(user.GetEffectiveAvatarUrl()))
                .WithEphemeral()
                .WithActionRows(
                    buttons.Take(2).AsActionRow(),
                    buttons.Skip(2).AsActionRow()
                ).RespondAsync();
        }
    }


    public sealed class PingCommand : SlashCommand
    {
        public PingCommand() : base("ping", "Show the Gateway and REST latency from my servers to Discord.") { }

        public override async Task HandleAsync(SlashCommandContext ctx)
        {
            var sw = Stopwatch.StartNew();
            await ctx.Backing.DeferAsync(true)
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

    public class PollCommand : SlashCommand
    {
        public PollCommand() : base("poll", "Create a poll.") { }

        public override SlashCommandBuilder GetCommandBuilder(IServiceProvider provider)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("question")
                    .WithDescription("What would you like to ask?")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("options")
                    .WithDescription("The options you want on the poll, separated by a semicolon (;). Limit 9.")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true));

        public override async Task HandleAsync(SlashCommandContext ctx)
        {
            var question = ctx.ValuedOptions["question"].Cast<string>();
            var options = ctx.ValuedOptions["options"].Cast<string>();

            if (PollInfo.TryParse($"{question};{options}", out var pollInfo))
            {
                await ctx.Backing.DeferAsync(true);
                Executor.Execute(async () =>
                {
                    await ctx.Backing.FollowupAsync(embed: pollInfo.Apply(ctx.CreateEmbedBuilder()).Build())
                        .Then(async m =>
                            await DiscordHelper.GetPollEmojis()[..pollInfo.Fields.Count]
                                .ForEachAsync(async emoji => await m.AddReactionAsync(emoji))
                        );
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

    public sealed class SpotifyCommand : SlashCommand
    {
        public SpotifyCommand() : base("spotify",
            "Shows what you're listening to on Spotify, if you're listening to something.") { }

        public override SlashCommandBuilder GetCommandBuilder(IServiceProvider provider)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("The member whose Spotify status you want to see. Defaults to yourself.")
                    .WithType(ApplicationCommandOptionType.User)
                    .WithRequired(false));

        public override async Task HandleAsync(SlashCommandContext ctx)
        {
            var target = ctx.Options["user"].GetValueOr<SocketGuildUser>(ctx.User);

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
                    .RespondAsync();
            }
        }
    }

    public sealed class SnowflakeCommand : SlashCommand
    {
        public SnowflakeCommand() : base("snowflake",
            "Shows when the object with the given Snowflake ID was created.") { }

        public override SlashCommandBuilder GetCommandBuilder(IServiceProvider provider)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("snowflake")
                    .WithDescription("The Discord snowflake you want to see.")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true));

        public override Task HandleAsync(SlashCommandContext ctx)
        {
            var id = ctx.ValuedOptions["snowflake"].Cast<string>();
            var reply = ctx.CreateReplyBuilder(true);

            if (!ulong.TryParse(id.Trim(), out var snowflake))
                reply.WithEmbeds(ctx.CreateEmbedBuilder().WithTitle("Input must be a number."));
            else
                reply.WithEmbeds(ctx.CreateEmbedBuilder()
                    .WithTitle(SnowflakeUtils.FromSnowflake(snowflake)
                        .GetDiscordTimestamp(TimestampType.LongDateTime)));

            return reply.RespondAsync();
        }
    }

    public sealed class InfoCommand : SlashCommand
    {
        public InfoCommand() : base("info", "Gets information for Discord things.") { }

        public override SlashCommandBuilder GetCommandBuilder(IServiceProvider _)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("guild")
                    .WithDescription("Show information about the current guild.")
                    .WithType(ApplicationCommandOptionType.SubCommand))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("bot")
                    .WithDescription("Show information about the current instance of Volte.")
                    .WithType(ApplicationCommandOptionType.SubCommand))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("Show information about a user, or yourself.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("target")
                        .WithDescription("The user to show information for.")
                        .WithType(ApplicationCommandOptionType.User)
                        .WithRequired(false)));

        public override async Task HandleAsync(SlashCommandContext ctx)
        {
            var subcommand = ctx.Options.First().Value;
            var reply = ctx.CreateReplyBuilder().WithEphemeral();

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
                    var target = (subcommand.Options ?? Array.Empty<SocketSlashCommandDataOption>()).FirstOrDefault()
                        .GetValueOr<SocketGuildUser>(ctx.User)!;

                    string GetRelevantActivity() => target.Activities.FirstOrDefault() switch
                    {
                        //we are ignoring custom emojis because there is no guarantee that volte is in the guild where the emoji is from; which could lead to a massive (and ugly) embed field value
                        CustomStatusGame { Emote: Emoji _ } csg => $"{csg.Emote} {csg.State}",
                        CustomStatusGame csg => $"{csg.State}",
                        SpotifyGame _ => "Listening to Spotify",
                        _ => target.Activities.FirstOrDefault()?.Name
                    } ?? "Nothing";

                    reply.WithEmbeds(ctx.CreateEmbedBuilder()
                        .WithTitle(target.ToString())
                        .AddField("ID", target.Id, true)
                        .AddField("Activity", GetRelevantActivity(), true)
                        .AddField("Status", target.Status, true)
                        .AddField("Is Bot", target.IsBot ? "Yes" : "No", true)
                        .AddField("Role Hierarchy", target.Hierarchy, true)
                        .AddField("Account Created",
                            $"{target.CreatedAt.GetDiscordTimestamp(TimestampType.LongDateTime)}")
                        .AddField("Joined This Guild",
                            $"{(target.JoinedAt.HasValue ? target.JoinedAt.Value.GetDiscordTimestamp(TimestampType.LongDateTime) : DiscordHelper.Zws)}")
                        .WithThumbnailUrl(target.GetEffectiveAvatarUrl(size: 512)));
                    break;
            }

            await reply.RespondAsync();
        }
    }
}