using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using Gommon;
using Humanizer;
using Qmmands;
using SixLabors.ImageSharp.PixelFormats;
using Volte.Commands.Results;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Core.Helpers;
using Volte.Core.Models.Misc;
using Volte.Services;

// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Commands.Modules
{
    public sealed class UtilityModule : VolteModule 
    {
        public CommandsService CommandsService { get; set; }
        public HttpService HttpService { get; set; }
        
        [Command("Tree")]
        [Description("Shows all categories in this guild and their children channels.")]
        [Remarks("tree")]
        public Task<ActionResult> TreeAsync()
        {
            var uncategorized = new StringBuilder().AppendLine(Formatter.Bold("Uncategorized"));
            var categories = new StringBuilder();

            foreach (var c in Context.Guild.GetTextChannels()
                .Where(c => c.ParentId == null)
                .Concat(Context.Guild.GetVoiceChannels()
                    .Where(a => a.ParentId == null)).OrderBy(c => c.Position))
            {
                if (CanSeeChannel(Context.Member, c))
                    uncategorized.AppendLine($"- {(c.Type is ChannelType.Voice ? "" : "#")}{c.Name}");
            }

            uncategorized.AppendLine();
            foreach (var category in Context.Guild.GetCategoryChannels().OrderBy(x => x.Position))
            {
                var categoryBuilder = new StringBuilder().AppendLine($"{Formatter.Bold(category.Name)}");
                foreach (var child in category.Children.OrderBy(c => c.Position))
                {
                    categoryBuilder.AppendLine($"- {(child.Type == ChannelType.Voice ? $"{child.Name}" : $"{child.Mention}")}");
                }
                categories.AppendLine(categoryBuilder.ToString());
            }

            var res = uncategorized.AppendLine(categories.ToString()).ToString();

            return res.Length >= 2048 // MaxDescriptionLength is hardcoded as 2048 in D#+ 
                ? BadRequest("This guild is too large; I cannot list all channels here.") 
                : Ok(res);
        }

        private readonly (char Key, string Value)[] _nato = new List<(char Key, string Value)>
        {
            ('a', "Alfa"), ('b', "Bravo"), ('c', "Charlie"), ('d', "Delta"),
            ('e', "Echo"), ('f', "Foxtrot"), ('g', "Golf"), ('h', "Hotel"),
            ('i', "India"), ('j', "Juliett"), ('k', "Kilo"), ('l', "Lima"),
            ('m', "Mike"), ('n', "November"), ('o', "Oscar"), ('p', "Papa"),
            ('q', "Quebec"), ('r', "Romeo"), ('s', "Sierra"), ('t', "Tango"),
            ('u', "Uniform"), ('v', "Victor"), ('w', "Whiskey"), ('x', "X-ray"),
            ('y', "Yankee"), ('z', "Zulu"), ('1', "One"), ('2', "Two"),
            ('3', "Three"), ('4', "Four"), ('5', "Five"), ('6', "Six"), 
            ('7', "Seven"), ('8', "Eight"), ('9', "Nine"), ('0', "Zero")

        }.ToArray();

        private string GetNato(char i) => 
            _nato.First(x => x.Key.ToString().EqualsIgnoreCase(i.ToString())).Value;

        private readonly string _baseWikiUrl = "https://github.com/Ultz/Volte/wiki";

        private (IOrderedEnumerable<(string Name, bool Value)> Allowed, IOrderedEnumerable<(string Name, bool Value)> Disallowed) GetPermissions(
            DiscordMember user)
        {
            var guildPermissions = user.GetGuildPermissions();
            var propDict = guildPermissions.GetType().GetProperties()
                .Where(a => a.PropertyType.Inherits<bool>())
                .Select(a => (a.Name.Humanize(), a.GetValue(guildPermissions).Cast<bool>()))
                .OrderByDescending(ab => ab.Item2 ? 1 : 0)
                .ToList(); //holy reflection

            return (propDict.Where(ab => ab.Item2).OrderBy(a => a.Item1), propDict.Where(ab => !ab.Item2).OrderBy(a => a.Item2));

        }

        private bool CanSeeChannel(DiscordMember member, DiscordChannel channel)
        {
            return member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels);
        }

        [Command("Wiki", "VolteWiki")]
        [Description("List all wiki pages or get a specific one in this one command.")]
        [Remarks("wiki [String]")]
        public Task<ActionResult> WikiAsync([Remainder, OptionalArgument] string page = null)
        {
            var embed = Context.CreateEmbedBuilder(string.Empty).WithThumbnail("https://i.greemdev.net/volte_whitepinkyellow.png");
            var pages = new Dictionary<string, string>
            { 
                { "Home", _baseWikiUrl },
                { "Features", $"{_baseWikiUrl}/Features"},
                { "Contributing", $"{_baseWikiUrl}/Contributing"},
                { "Setting Volte Up", $"{_baseWikiUrl}/Setting-Volte-Up"},
                { "Argument Cheatsheet", $"{_baseWikiUrl}/Argument-Cheatsheet"},
                { "Developers:Selfhost:Windows", $"{_baseWikiUrl}/Windows"},
                { "Developers:Selfhost:Linux", $"{_baseWikiUrl}/Linux"},
                { "Developers:Dependency Injection", $"{_baseWikiUrl}/Dependency-Injection"}
            };

            if (page is null)
            {
                return Ok(embed.WithDescription(FormatPages()));
            }

            return Ok(embed.WithDescription(pages.ContainsKey(page)
                ? $"[{pages.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(page))}]({pages.FirstOrDefault(x => x.Key.EqualsIgnoreCase(page)).Value})"
                : $"{page} wasn't found. Here's a list of valid wiki pages: {FormatPages()}"));


            string FormatPages()
            {
                var formattedPages = new StringBuilder();
                foreach (var (key, value) in pages)
                {
                    formattedPages.AppendLine($"[{key}]({value})");
                }

                return formattedPages.ToString();
            }
        }

        [Command("Uptime")]
        [Description("Shows the bot's uptime in a human-friendly fashion.")]
        [Remarks("uptime")]
        public Task<ActionResult> UptimeAsync() 
            => Ok($"I've been online for **{Process.GetCurrentProcess().CalculateUptime()}**!");

        [Command("Suggest")]
        [Description("Suggest features for Volte.")]
        [Remarks("suggest")]
        public Task<ActionResult> SuggestAsync() 
            => Ok("You can suggest bot features [here](https://goo.gl/forms/i6pgYTSnDdMMNLZU2).");

        [Command("Snowflake")]
        [Description("Shows when the object with the given Snowflake ID was created, in UTC.")]
        [Remarks("snowflake {Ulong}")]
        public Task<ActionResult> SnowflakeAsync([RequiredArgument] ulong id)
        {
            var date = id.FromDiscordSnowflake();
            return Ok(new StringBuilder()
                .AppendLine($"**Date:** {date.FormatDate()}")
                .AppendLine($"**Time**: {date.FormatFullTime()}")
                .ToString());
        }

        [Command("ShowColor", "Sc")]
        [Description("Shows an image purely made up of the specified color.")]
        [Remarks("showcolor {Color}")]
        public Task<ActionResult> ShowColorAsync([Remainder, RequiredArgument] DiscordColor color)
            => Ok(async () =>
            {
                await using var stream = new Rgba32(color.R, color.G, color.B).CreateColorImage();
                await stream.SendFileToAsync("role.png", Context.Channel, embed: new DiscordEmbedBuilder()
                    .WithColor(color)
                    .WithTitle($"Color {color}")
                    .WithDescription(new StringBuilder()
                        .AppendLine($"**Hex:** {color.ToString().ToUpper()}")
                        .AppendLine($"**RGB:** {color.R}, {color.G}, {color.B}")
                        .ToString())
                    .WithImageUrl("attachment://role.png")
                    .WithCurrentTimestamp()
                    .Build());
            });

        [Command("Say")]
        [Description("Bot repeats what you tell it to.")]
        [Remarks("say {String}")]
        public Task<ActionResult> SayAsync([Remainder, RequiredArgument] string msg) 
            => Ok(msg, _ => Context.Message.DeleteAsync());

        [Command("SilentSay", "SSay")]
        [Description("Runs the say command normally, but doesn't show the author in the message. Useful for announcements.")]
        [Remarks("silentsay {String}")]
        public Task<ActionResult> SilentSayAsync([Remainder, RequiredArgument] string msg) 
            => Ok(new DiscordEmbedBuilder()
                .WithColor(Context.Member.GetHighestRoleWithColor()?.Color ?? new DiscordColor(Config.SuccessColor))
                .WithDescription(msg), _ => Context.Message.DeleteAsync());

        [Command("Quote"), Priority(0)]
        [Description("Quotes a user from a given message's ID.")]
        [Remarks("quote {Ulong}")]
        public async Task<ActionResult> QuoteAsync([RequiredArgument] ulong messageId)
        {
            var m = await Context.Channel.GetMessageAsync(messageId);
            if (m is null)
                return BadRequest("A message with that ID doesn't exist in this channel.");

            var e = Context.CreateEmbedBuilder(new StringBuilder()
                    .AppendLine($"{m.Content}")
                    .AppendLine()
                    .AppendLine($"[Jump!]({m.JumpLink})")
                    .ToString())
                .WithAuthor($"{m.Author}, in #{m.Channel.Name}",
                    m.Author.GetAvatarUrl(ImageFormat.Auto))
                .WithFooter(m.Timestamp.Humanize());
            if (!m.Attachments.IsEmpty())
            {
                e.WithImageUrl(m.Attachments.FirstOrDefault()?.Url);
            }

            return Ok(e);
        }

        [Command("Quote"), Priority(1)]
        [Description("Quotes a user in a different chanel from a given message's ID.")]
        [Remarks("quote {messageId}")]
        public async Task<ActionResult> QuoteAsync([RequiredArgument] DiscordChannel channel, [RequiredArgument] ulong messageId)
        {
            var m = await channel.GetMessageAsync(messageId);
            if (m is null)
                return BadRequest("A message with that ID doesn't exist in the given channel.");

            var e = Context.CreateEmbedBuilder(new StringBuilder()
                    .AppendLine($"{m.Content}")
                    .AppendLine()
                    .AppendLine($"[Jump!]({m.JumpLink})")
                    .ToString())
                .WithAuthor($"{m.Author}, in #{m.Channel.Name}",
                    m.Author.GetAvatarUrl(ImageFormat.Auto))
                .WithFooter(m.Timestamp.Humanize());
            if (!m.Attachments.IsEmpty())
            {
                e.WithImageUrl(m.Attachments[0].Url);
            }

            return Ok(e);
        }

        [Command("Prefix")]
        [Description("Shows the command prefix for this guild.")]
        [Remarks("prefix")]
        public Task<ActionResult> PrefixAsync() 
            => Ok($"The prefix for this guild is **{Context.GuildData.Configuration.CommandPrefix}**; " +
                  $"alternatively you can just mention me as a prefix, i.e. `{Context.Guild.CurrentMember.Mention} help`.");

        [Command("Poll")]
        [Description("Create a poll. The 'String' argument must be in the form of question;option1[;option2;option3;option4;option5].")]
        [Remarks("poll {TimeSpan} {String}")]
        public Task<ActionResult> PollAsync([RequiredArgument] TimeSpan duration, [Remainder, RequiredArgument] string all)
        {
            var content = all.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var pollInfo = GetPollBody(content);
            var choicesCount = content.Length - 1;
            if (!pollInfo.IsValid)
                return BadRequest(choicesCount > 5
                    ? "More than 5 options were specified."
                    : "No options specified.");

            var embed = Context.CreateEmbedBuilder()
                .WithTitle(Formatter.Bold(content[0]));

            foreach (var (key, value) in pollInfo.Fields)
            {
                embed.AddField(key, value, true);
            }

            return Ok(embed.WithFooter($"This poll will end in {duration.Humanize(3)}"), async msg =>
            {
                var result = await Context.Interactivity.DoPollAsync(msg, EmojiHelper.GetPollEmojisList().Take(choicesCount).ToArray(), PollBehaviour.KeepEmojis, duration);
                embed = embed.WithTitle("Poll Ended! Here are the results:");
                foreach (var res in result.OrderByDescending(x => x.Total))
                {
                    embed.AddField(res.Emoji.Name, res.Total);
                }

                await Context.ReplyAsync(embed);

            }, false);
        }

        [Command("Ping")]
        [Description("Show the Gateway latency to Discord.")]
        [Remarks("ping")]
        public Task<ActionResult> PingAsync()
            => None(async () =>
            {
                var e = Context.CreateEmbedBuilder("Pinging...");
                var sw = new Stopwatch();
                sw.Start();
                var msg = await e.SendToAsync(Context.Channel);
                sw.Stop();
                await msg.ModifyAsync(embed: e.WithDescription(new StringBuilder()
                        .AppendLine($"{EmojiHelper.Clap} **Gateway**: {Context.Client.GetMeanLatency()} milliseconds")
                        .AppendLine($"{EmojiHelper.OkHand} **REST**: {sw.Elapsed.Humanize(3)}")
                        .ToString())
                    .Build());
            }, false);

        [Command("Permissions", "Perms")]
        [Description("Shows someone's, or the command invoker's, permissions in the current guild.")]
        [Remarks("permissions [User]")]
        public Task<ActionResult> PermissionsAsync([Remainder, OptionalArgument] DiscordMember user = null)
        {
            user ??= Context.Member; // get the user (or the invoker, if none specified)


            if (user.Id == Context.Guild.Owner.Id)
            {
                return Ok("User is owner of this guild, and has all permissions.");
            }
            if (user.GetGuildPermissions().HasPermission(Permissions.Administrator))
            {
                return Ok("User has Administrator permission, and has all permissions.");
            }


            var (allowed, disallowed) = GetPermissions(user);

            var allowedString = allowed.Select(a => $"- {a.Name}").Join('\n');
            var disallowedString = disallowed.Select(a => $"- {a.Name}").Join('\n');
            return Ok(Context.CreateEmbedBuilder().WithAuthor(user)
                .AddField("Allowed", allowedString.IsNullOrEmpty() ? "- None" : allowedString, true)
                .AddField("Denied", disallowedString.IsNullOrEmpty() ? "- None" : disallowedString, true));
        }

        [Command("Now")]
        [Description("Shows the current date and time, in UTC.")]
        [Remarks("now")]
        public Task<ActionResult> NowAsync()
            => Ok(new DiscordEmbedBuilder().WithDescription(new StringBuilder()
                .AppendLine($"**Date**: {Context.Now.FormatDate()} UTC")
                .AppendLine($"**Time**: {Context.Now.FormatFullTime()} UTC")
                .ToString()).WithCurrentTimestamp());

        [Command("Nato")]
        [Description("Translates a string into the NATO Phonetic Alphabet. If no string is provided, then a full rundown of the NATO alphabet is shown.")]
        [Remarks("nato [String]")]
        public Task<ActionResult> NatoAsync([Remainder, OptionalArgument] string input = null)
        {
            if (input.IsNullOrEmpty())
                return None(async () =>
                {
                    await Context.Interactivity.SendPaginatedMessageAsync(Context.Channel, Context.Member,
                        _nato.Select(x => $"**{x.Key.ToString().ToUpper()}**: `{x.Value}`").GetPages(10));
                }, false);

            // ReSharper disable once PossibleNullReferenceException
            //this legit cant happen because of the if statement above
            var arr = input.ToLower().ToCharArray().Where(x => !x.Equals(' '));
            var l = new List<string>();

            foreach (var ch in arr)
            {
                try
                {
                    l.Add(GetNato(ch));
                }
                catch (InvalidOperationException)
                {
                    return BadRequest(
                        $"There is not a NATO word for the character `{ch}`. Only standard English letters and numbers are valid.");
                }
            }

            return Ok(Context.CreateEmbedBuilder().AddField("Result", $"`{l.Join(" ")}`")
                .AddField("Original", $"`{input}`"));
        }

        [Command("Invite")]
        [Description("Get an invite to use Volte in your own guild.")]
        [Remarks("invite")]
        public Task<ActionResult> InviteAsync()
            => Ok(new StringBuilder()
                .AppendLine(
                    "Do you like Volte? If you do, that's awesome! If not then I'm sorry (please tell me what you don't like [here](https://forms.gle/CJ9XtKmKf2Q2mQwb7)!) :( ")
                .AppendLine()
                .AppendLine("[Website](https://greemdev.net/Volte)")
                .AppendLine($"[Invite Me]({Context.Client.GetInviteUrl()})")
                .AppendLine("[Support Guild Invite](https://discord.gg/H8bcFr2)")
                .AppendLine()
                .AppendLine("And again, thanks for using me!")
                .ToString());

        [Command("Info")]
        [Description("Provides basic information about this instance of Volte.")]
        [Remarks("info")]
        public async Task<ActionResult> InfoAsync()
            => Ok(Context.CreateEmbedBuilder()
                .AddField("Version", Version.FullVersion, true)
                .AddField("Author", $"{await Context.Client.ShardClients.First().Value.GetUserAsync(168548441939509248)}, contributors on [GitHub](https://github.com/Ultz/Volte), and members of the Ultz organization.", true)
                .AddField("Language/Library", $"C# 9, DSharpPlus {Version.DSharpPlusVersion}", true)
                .AddField("Guilds", Context.Client.GetGuildCount(), true)
                .AddField("Shards", Context.Client.ShardClients.Count, true)
                .AddField("Channels", Context.Client.GetChannelCount(), true) // TODO grossly oversimplified for now
                .AddField("Invite Me", $"`{CommandService.GetCommand("Invite").GetUsage(Context)}`", true)
                .AddField("Uptime", Process.GetCurrentProcess().CalculateUptime(), true)
                .AddField("Successful Commands", CommandsService.SuccessfulCommandCalls, true)
                .AddField("Failed Commands", CommandsService.FailedCommandCalls, true)
                .WithThumbnail(Context.Client.CurrentUser.AvatarUrl));

        [Command("UserInfo", "Ui")]
        [Description("Shows info for the mentioned user or yourself if none is provided.")]
        [Remarks("userinfo [user]")]
        public Task<ActionResult> UserInfoAsync([Remainder, OptionalArgument] DiscordMember user = null)
        {
            user ??= Context.Member;

            return Ok(Context.CreateEmbedBuilder()
                .WithThumbnail(user.AvatarUrl)
                .WithTitle("User Info")
                .AddField("User ID", user.Id)
                .AddField("Is Bot", user.IsBot)
                .AddField("Account Created",
                    $"{user.CreationTimestamp.FormatDate()}, {user.CreationTimestamp.FormatFullTime()}")
                .AddField("Joined This Guild",
                    $"{(user.JoinedAt != default ? user.JoinedAt.FormatDate() : "\u200B")}, " +
                    $"{(user.JoinedAt != default ? user.JoinedAt.FormatFullTime() : "\u200B")}")
                .WithThumbnail(user.GetAvatarUrl(ImageFormat.Auto, 512)));
        }

        [Command("GuildInfo", "Gi")]
        [Description("Shows some info about the current guild.")]
        [Remarks("guildinfo")]
        public Task<ActionResult> GuildInfoAsync()
        {
            var cAt = Context.Guild.CreationTimestamp;

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle("Guild Info")
                .WithThumbnail(Context.Guild.IconUrl)
                .AddField("Name", Context.Guild.Name)
                .AddField("Created", $"{cAt.Month}.{cAt.Day}.{cAt.Year} ({cAt.Humanize()})")
                .AddField("Region", Context.Guild.VoiceRegion.Id)
                .AddField("Members", Context.Guild.MemberCount, true)
                .AddField("Roles", Context.Guild.Roles.Count, true)
                .AddField("Category Channels", Context.Guild.GetCategoryChannels().Count(), true)
                .AddField("Voice Channels", Context.Guild.GetVoiceChannels().Count(), true)
                .AddField("Text Channels", Context.Guild.GetTextChannels().Count(), true)
                .WithThumbnail(Context.Guild.IconUrl));
        }

        [Command("IamNot")]
        [Description("Take a role from yourself, if it is in the current guild's self role list.")]
        [Remarks("iamnot {String}")]
        public async Task<ActionResult> IamNotAsync([Remainder, RequiredArgument] DiscordRole role)
        {
            if (!Context.GuildData.Extras.SelfRoles.Any(x => x.EqualsIgnoreCase(role.Name)))
            {
                return BadRequest($"The role **{role.Name}** isn't in the self roles list for this guild.");
            }

            Context.Guild.Roles.FirstOrDefault(x => x.Value.Name.EqualsIgnoreCase(role.Name)).Deconstruct(out _, out var target);
            
            if (target is null)
            {
                return BadRequest($"The role **{role.Name}** doesn't exist in this guild.");
            }

            await Context.Member.RevokeRoleAsync(target);
            return Ok($"Took away your **{role.Name}** role.");
        }

        [Command("Hastebin")]
        [Description("Posts the provided content to https://paste.greemdev.net and returns the URL so you can send it to people.")]
        [Remarks("hastebin {String}")]
        public async Task<ActionResult> HastebinAsync([Remainder, RequiredArgument] string content)
        {
            var url = await HttpService.PostToGreemPasteAsync(content);
            return Ok($"Click [here]({url}) to see the paste!");
        }

        [Command("Iam")]
        [Description("Gives yourself a role, if it is in the current guild's self role list.")]
        [Remarks("iam {String}")]
        public async Task<ActionResult> IamAsync([Remainder, RequiredArgument] DiscordRole role)
        {
            if (Context.GuildData.Extras.SelfRoles.IsEmpty())
            {
                return BadRequest("This guild does not have any roles you can give yourself.");
            }

            var target = Context.Guild.Roles.FirstOrDefault(x => x.Value.Name.EqualsIgnoreCase(role.Name)).Value;
            
            if (!Context.GuildData.Extras.SelfRoles.Any(x => x.EqualsIgnoreCase(role.Name)))
            {
                return BadRequest($"The role **{role.Name}** isn't in the self roles list for this guild.");
            }
            
            if (target is null)
            {
                return BadRequest($"The role **{role.Name}** doesn't exist in this guild.");
            }

            await Context.Member.GrantRoleAsync(target);
            return Ok($"Gave you the **{role.Name}** role.");
        }

        [Command("Feedback", "Fb")]
        [Description("Submit feedback directly to the Volte guild. Won't work on non-public Volte.")]
        [Remarks("feedback {String}")]
        [EnsurePublicVolte]
        public Task<ActionResult> FeedbackAsync([Remainder]string feedback)
            => Ok($"Feedback sent! Message: ```{feedback}```", _ =>
                Context.CreateEmbedBuilder($"```{feedback}```")
                    .WithTitle($"Feedback from {Context.Member}")
                    .SendToAsync(Context.Client.GetPrimaryGuild().GetChannel(415182876326232064))
            );

        [Command("Color", "Colour")]
        [Description("Shows the Hex and RGB representation for a given role in the current guild.")]
        [Remarks("color {Role}")]
        public async Task<ActionResult> RoleColorAsync([Remainder, RequiredArgument] DiscordRole role)
        {
            if (!role.HasColor()) return BadRequest("Role does not have a color.");
            
            await using var stream = new Rgba32(role.Color.R, role.Color.G, role.Color.B).CreateColorImage();
            await stream.SendFileToAsync("color.png", Context.Channel, embed: new DiscordEmbedBuilder()
                .WithColor(role.Color)
                .WithTitle("Role Color")
                .WithDescription(new StringBuilder()
                    .AppendLine($"**Hex:** {role.Color.ToString().ToUpper()}")
                    .AppendLine($"**RGB:** {role.Color.R}, {role.Color.G}, {role.Color.B}")
                    .ToString())
                .WithImageUrl("attachment://role.png")
                .WithCurrentTimestamp()
                .Build());
            return None();

        }

        [Command("Choose")]
        [Description("Choose an item from a list separated by |.")]
        [Remarks("choose {String}")]
        public Task<ActionResult> ChooseAsync([Remainder, RequiredArgument("option1|option2|option3|...")] string options) 
            => Ok($"I choose `{options.Split('|', StringSplitOptions.RemoveEmptyEntries).Random()}`.");

        [Command("BigEmoji", "HugeEmoji", "BigEmote", "HugeEmote")]
        [Description("Shows the image URL for a given emoji.")]
        [Remarks("bigemoji {Emote}")]
        public Task<ActionResult> BigEmojiAsync([RequiredArgument] DiscordEmoji emoteIn)
        {
            string url = null;
            try
            {
                url =
                    $"https://i.kuro.mu/emoji/512x512/{emoteIn?.ToString().GetUnicodePoints().Select(x => x.ToString("x2")).Join('-')}.png";
            }
            catch (ArgumentNullException)
            { }

            return emoteIn switch
            {
                { Id: not 0 } emote => Ok(Context.CreateEmbedBuilder(emote.Url).WithImageUrl(emote.Url)),
                { Id: 0 } _ => Ok(Context.CreateEmbedBuilder(url).WithImageUrl(url)),
                _ => None() //should never be reached
            };
        }

        [Command("Avatar")]
        [Description("Shows the mentioned user's avatar, or yours if no one is mentioned.")]
        [Remarks("avatar [User]")]
        public Task<ActionResult> AvatarAsync([Remainder, OptionalArgument] DiscordMember user = null)
        {
            user ??= Context.Member;
            return Ok(Context.CreateEmbedBuilder()
                .WithAuthor(user)
                .WithImageUrl(user.GetAvatarUrl(ImageFormat.Auto)));
        }
        
        private PollInfo GetPollBody(IEnumerable<string> choices)
        {
            var c = choices as string[] ?? choices.ToArray();
            return PollInfo.FromDefaultFields(c.Length - 1, c);
        }
    }
}