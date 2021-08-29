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

namespace Volte.Commands.Interaction.Commands
{
    public sealed class IamCommand : ApplicationCommand
    {
        public IamCommand() : base("iam", "Give yourself Self Roles via dropdown menu.") { }

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider provider) => new SlashCommandBuilder();

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
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

    public sealed class IamNotCommand : ApplicationCommand
    {
        public IamNotCommand() : base("iamnot", "Take away Self Roles from yourself via dropdown menu.") { }

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider provider) => new SlashCommandBuilder();

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
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

            await ctx.GuildUser.RemoveRolesAsync(
                ctx.SelectedMenuOptions.Select(x => ctx.Guild.GetRole(ulong.Parse(x))));
        }
    }

    public class CountMembersCommand : ApplicationCommand
    {
        public CountMembersCommand() : base("count-members",
            "Counts the amount of members in the current guild have the provided role.") { }

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider provider)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("role")
                    .WithDescription("The role to count members in.")
                    .WithType(ApplicationCommandOptionType.Role)
                    .WithRequired(true));

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
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

    public sealed class UptimeCommand : ApplicationCommand
    {
        public UptimeCommand() : base("uptime", "Shows the bot's uptime.") { }

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider provider) => new SlashCommandBuilder();

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
            => await ctx.CreateReplyBuilder(true)
                .WithEmbed(e => e.WithTitle(Process.GetCurrentProcess().CalculateUptime()))
                .RespondAsync();
    }

    public class AvatarCommand : ApplicationCommand
    {
        public AvatarCommand() : base("avatar", "Gets the avatar of the desired user or yourself.") { }

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider _)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithType(ApplicationCommandOptionType.User)
                    .WithName("user")
                    .WithDescription("The user's avatar you want to see.")
                    .WithRequired(false));

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var user = ctx.Options["user"].GetValueOr<SocketGuildUser>(ctx.User);
            var buttons = new ushort[] { 128, 256, 512, 1024 }
                .Select(x => ButtonBuilder.CreateLinkButton($"{x}x{x}", user.GetEffectiveAvatarUrl(size: x)).Build())
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


    public sealed class PingCommand : ApplicationCommand
    {
        public PingCommand() : base("ping", "Show the Gateway and REST latency from my servers to Discord.") { }

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider provider) => new SlashCommandBuilder();

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
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

    public class PollCommand : ApplicationCommand
    {
        public PollCommand() : base("poll", "Create a poll.") { }

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider provider)
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

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
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

    public sealed class SpotifyCommand : ApplicationCommand
    {
        public SpotifyCommand() : base("spotify",
            "Shows what you're listening to on Spotify, if you're listening to something.") { }

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider provider)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("The member whose Spotify status you want to see. Defaults to yourself.")
                    .WithType(ApplicationCommandOptionType.User)
                    .WithRequired(false));

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
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

    public sealed class SnowflakeCommand : ApplicationCommand
    {
        public SnowflakeCommand() : base("snowflake",
            "Shows when the object with the given Snowflake ID was created.") { }

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider provider)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("snowflake")
                    .WithDescription("The Discord snowflake you want to see.")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true));

        public override Task HandleSlashCommandAsync(SlashCommandContext ctx)
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


        public InfoCommand() : base("info", "Gets information for Discord things.") { }

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider _)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("guild")
                    .WithDescription("Show information about the current guild.")
                    .WithType(ApplicationCommandOptionType.SubCommand))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("invite")
                    .WithDescription("Get useful links for Volte such as the GitHub and Invite URL.")
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
                    var target = (subcommand.Options ?? Array.Empty<SocketSlashCommandDataOption>()).FirstOrDefault()
                        .GetValueOr<SocketGuildUser>(ctx.User)!;

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