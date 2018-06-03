using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Bot.Internal;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Modules.Utilities
{
    public class Utils : ModuleBase<SocketCommandContext>
    {
        [Command("Avatar")]
        public async Task GetUsersAvatar(SocketGuildUser s)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithImageUrl(s.GetAvatarUrl())
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username))
                .WithDescription($"**{s.Username}#{s.DiscriminatorValue}**'s Avatar");
            await ReplyAsync("", false, embed);
        }

        [Command("UserInfo")]
        [Alias("uinfo", "useri", "ui")]
        public async Task UserInformationCommand(SocketGuildUser user = null)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            if (user != null)
            {
                embed.AddField("Username", $"{user.Username}#{user.Discriminator}");
                embed.AddField("User ID", user.Id);
                if (user.Game != null)
                    embed.AddField("Game", user.Game);
                else
                    embed.AddField("Game", "Nothing");
                embed.AddField("Status", user.Status);
                embed.AddField("Account Created", user.CreatedAt.UtcDateTime);
                embed.WithThumbnailUrl(user.GetAvatarUrl());
                embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
                embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
                embed.WithTitle("User Information");
                embed.AddField("Is Bot", user.IsBot);
            }
            else
            {
                embed.AddField("Username", $"{Context.User.Username}#{Context.User.Discriminator}");
                embed.AddField("User ID", Context.User.Id);
                if (Context.User.Game != null)
                    embed.AddField("Game", Context.User.Game);
                else
                    embed.AddField("Game", "Nothing");
                embed.AddField("Status", Context.User.Status);
                embed.AddField("Account Created", Context.User.CreatedAt.UtcDateTime);
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
                embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
                embed.WithTitle("User Information");
                embed.AddField("Is Bot", Context.User.IsBot);
            }


            await ReplyAsync("", false, embed);
        }

        [Command("Feedback")]
        [Alias("Fb")]
        public async Task SendFeedbackToDev([Remainder] string feedback)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithDescription(Bot.Internal.Utilities.GetFormattedLocaleMsg("FeedbackCommandText", feedback));
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithTitle("Feedback to Greem");
            await ReplyAsync("", false, embed);
            var feedbackEmbed = new EmbedBuilder()
                .WithDescription(feedback)
                .WithTitle($"Feedback from {Context.User.Username}#{Context.User.DiscriminatorValue}")
                .WithColor(Config.bot.DefaultEmbedColour);


            var channel = Program._client.GetGuild(405806471578648588).GetTextChannel(415182876326232064);
            await channel.SendMessageAsync("", false, feedbackEmbed);
        }

        [Command("Calculator")]
        [Alias("Calc")]
        public async Task
            Calculate(string oper, int val1, int val2 = 0) //this code is fucking nasty, i will fix it in the future.
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithTitle("Calculator");

            int result;
            double result2;

            switch (oper)
            {
                case "add":
                    result = val1 + val2;
                    embed.WithDescription($"The answer is `{result}`");
                    break;
                case "sub":
                    result = val1 - val2;
                    embed.WithDescription($"The answer is `{result}`");
                    break;
                case "mult":
                    result2 = Math.BigMul(val1, val2);
                    embed.WithDescription($"The answer is `{result2}`");
                    break;
                case "div":
                    Math.DivRem(val1, val2, out var b);
                    embed.WithDescription($"The answer is `{b}`");
                    break;
                case "sqrt":
                    result2 = Math.Sqrt(val1);
                    embed.WithDescription($"The answer is `{result2}`");
                    break;
                case "power":
                    result2 = Math.Pow(val1, val2);
                    embed.WithDescription($"The answer is `{result2}`");
                    break;
                default:
                    embed.WithDescription(
                        "You didn't specify a valid operation. Valid operations are `add`, `sub`, `mult`, `div`, `power`, and `sqrt`.");
                    break;
            }

            await ReplyAsync("", false, embed);
        }

        [Command("YouTube")]
        [Alias("Yt")]
        public async Task SearchYouTube([Remainder] string query)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithThumbnailUrl("https://www.freepnglogos.com/uploads/youtube-logo-hd-8.png");

            var url = "https://youtube.com/results?search_query=";
            var newQuery = query.Replace(' ', '+');
            embed.WithDescription(url + newQuery);

            await ReplyAsync("", false, embed);
        }

        [Command("ServerInfo")]
        [Alias("sinfo", "serveri", "si")]
        public async Task ServerInformationCommand()
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithTitle("Server Information");
            embed.AddField("Name", Context.Guild.Name);
            embed.AddField("Created", Context.Guild.CreatedAt.UtcDateTime);
            embed.AddField("Users", Context.Guild.Users.Count);
            embed.AddField("Text Channels", Context.Guild.TextChannels.Count);
            embed.AddField("Voice Channels", Context.Guild.VoiceChannels.Count);
            embed.AddField("Region", Context.Guild.VoiceRegionId);
            embed.WithThumbnailUrl(Context.Guild.IconUrl);
            embed.AddField("Roles", Context.Guild.Roles.Count);
            embed.AddField("Donator Guild", config.VerifiedGuild);
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));

            await ReplyAsync("", false, embed);
        }

        [Command("Iam")]
        public async Task GiveYourselfRole([Remainder] string role)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var user = Context.User as SocketGuildUser;
            var embed = new EmbedBuilder()
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            if (config == null)
            {
                embed.WithDescription("This server doesn't have any self roles set.");
            }
            else
            {
                if (config.SelfRoles.Contains(role))
                {
                    embed.WithDescription($"Gave you the **{role}** role.");
                    var r = Context.Guild.Roles.FirstOrDefault(x => x.Name == role);
                    await user.AddRoleAsync(r);
                }
                else
                {
                    embed.WithDescription(
                        "That role isn't in the self roles list for this server. Remember that this command is cAsE sEnSiTiVe!");
                }
            }

            await ReplyAsync("", false, embed);
        }

        [Command("Iamnot")]
        [Alias("Iamn")]
        public async Task TakeAwayRole([Remainder] string role)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            var user = Context.User as SocketGuildUser;
            var embed = new EmbedBuilder()
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            if (config == null)
            {
                embed.WithDescription("This server doesn't have any self roles set.");
            }
            else
            {
                if (config.SelfRoles.Contains(role))
                {
                    embed.WithDescription($"Removed your **{role}** role.");
                    var r = Context.Guild.Roles.FirstOrDefault(x => x.Name == role);
                    await user.RemoveRoleAsync(r);
                }
                else
                {
                    embed.WithDescription(
                        "That role isn't in the self roles list for this server. Remember that this command is cAsE sEnSiTiVe!");
                }
            }

            await ReplyAsync("", false, embed);
        }

        [Command("CustomCommandList")]
        [Alias("Ccl")]
        public async Task GetCustomCommandsForServer()
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            var commandList = "";
            foreach (var value in config.CustomCommands.Keys) commandList += $"**{value}**\n";
            var embed = new EmbedBuilder()
                .WithDescription(commandList)
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithTitle($"Custom Commands available for {Context.Guild.Name}")
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            await ReplyAsync("", false, embed);
        }

        [Command("SelfRoleList")]
        [Alias("Srl")]
        public async Task GetSelfRoleListForServer()
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            if (config == null)
            {
                embed.WithDescription("This server doesn't have any self-assignable roles.");
            }
            else
            {
                config.SelfRoles.Sort();
                var roles = "\n";
                foreach (var role in config.SelfRoles) roles += $"**{role}**\n";

                embed.WithTitle("Roles you can self-assign: ");
                embed.WithDescription(roles);
            }

            await ReplyAsync("", false, embed);
        }


        [Command("Poll")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task ReactionPoll([Remainder] string pollBody)
        {
            var choices = pollBody.Split(';');
            var numbers = choices.Length - 1;

            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithTitle(choices[0])
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithAuthor(Context.User)
                .WithThumbnailUrl("http://survation.com/wp-content/uploads/2016/09/polleverywherelogo.png");

            switch (numbers)
            {
                case 1:
                    embed.WithDescription($"{new Emoji("🇦")} {choices[1]}\n\n" +
                                          "Click one of the letters below to vote.");
                    break;
                case 2:
                    embed.WithDescription($"{new Emoji("🇦")} {choices[1]}\n" +
                                          $"{new Emoji("🇧")} {choices[2]}\n\n" +
                                          "Click one of the letters below to vote.");
                    break;
                case 3:
                    embed.WithDescription($"{new Emoji("🇦")} {choices[1]}\n" +
                                          $"{new Emoji("🇧")} {choices[2]}\n" +
                                          $"{new Emoji("🇨")} {choices[3]}\n\n" +
                                          "Click one of the letters below to vote.");
                    break;

                case 4:
                    embed.WithDescription($"{new Emoji("🇦")} {choices[1]}\n" +
                                          $"{new Emoji("🇧")} {choices[2]}\n" +
                                          $"{new Emoji("🇨")} {choices[3]}\n" +
                                          $"{new Emoji("🇩")} {choices[4]}\n\n" +
                                          "Click one of the letters below to vote.");
                    break;
                case 5:
                    embed.WithDescription($"{new Emoji("🇦")} {choices[1]}\n" +
                                          $"{new Emoji("🇧")} {choices[2]}\n" +
                                          $"{new Emoji("🇨")} {choices[3]}\n" +
                                          $"{new Emoji("🇩")} {choices[4]}\n" +
                                          $"{new Emoji("🇪")} {choices[5]}\n\n" +
                                          "Click one of the letters below to vote.");
                    break;
                case 6:
                    embed.WithDescription($"{new Emoji("🇦")} {choices[1]}\n" +
                                          $"{new Emoji("🇧")} {choices[2]}\n" +
                                          $"{new Emoji("🇨")} {choices[3]}\n" +
                                          $"{new Emoji("🇩")} {choices[4]}\n" +
                                          $"{new Emoji("🇪")} {choices[5]}\n" +
                                          $"{new Emoji("🇫")} {choices[6]}\n\n" +
                                          "Click one of the letters below to vote.");
                    break;
                case 7:
                    embed.WithDescription($"{new Emoji("🇦")} {choices[1]}\n" +
                                          $"{new Emoji("🇧")} {choices[2]}\n" +
                                          $"{new Emoji("🇨")} {choices[3]}\n" +
                                          $"{new Emoji("🇩")} {choices[4]}\n" +
                                          $"{new Emoji("🇪")} {choices[5]}\n" +
                                          $"{new Emoji("🇫")} {choices[6]}\n" +
                                          $"{new Emoji("🇬")} {choices[7]}\n\n" +
                                          "Click one of the letters below to vote.");
                    break;
                case 8:
                    embed.WithDescription($"{new Emoji("🇦")} {choices[1]}\n" +
                                          $"{new Emoji("🇧")} {choices[2]}\n" +
                                          $"{new Emoji("🇨")} {choices[3]}\n" +
                                          $"{new Emoji("🇩")} {choices[4]}\n" +
                                          $"{new Emoji("🇪")} {choices[5]}\n" +
                                          $"{new Emoji("🇫")} {choices[6]}\n" +
                                          $"{new Emoji("🇬")} {choices[7]}\n" +
                                          $"{new Emoji("🇭")} {choices[8]}\n\n" +
                                          "Click one of the letters below to vote.");
                    break;

                default:
                    embed.WithDescription("No options specified.");
                    break;
            }

            if (choices.Length > 8)
            {
                embed.WithDescription("You cannot have more than 8 options.");
                await ReplyAsync("", false, embed);
                return;
            }

            var msg = await ReplyAsync("", false, embed);
            await Context.Message.DeleteAsync();

            switch (numbers)
            {
                case 1:
                    await msg.AddReactionAsync(new Emoji("🇦"));
                    break;
                case 2:
                    await msg.AddReactionAsync(new Emoji("🇦"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇧"));
                    break;
                case 3:
                    await msg.AddReactionAsync(new Emoji("🇦"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇧"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇨"));
                    break;
                case 4:
                    await msg.AddReactionAsync(new Emoji("🇦"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇧"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇨"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇩"));
                    break;
                case 5:
                    await msg.AddReactionAsync(new Emoji("🇦"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇧"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇨"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇩"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇪"));
                    break;
                case 6:
                    await msg.AddReactionAsync(new Emoji("🇦"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇧"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇨"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇪"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇫"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇦"));
                    break;
                case 7:
                    await msg.AddReactionAsync(new Emoji("🇦"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇧"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇨"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇩"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇪"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇫"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇬"));
                    break;
                case 8:
                    await msg.AddReactionAsync(new Emoji("🇦"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇧"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇨"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇩"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇪"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇫"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇬"));
                    Thread.Sleep(500);
                    await msg.AddReactionAsync(new Emoji("🇭"));
                    break;
            }
        }

        [Command("Lmgtfy")]
        [Alias("Googleit")]
        public async Task WhyDoYouBotherMeLol([Remainder] string oh)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            oh = oh.Replace(' ', '+');
            var embed = new EmbedBuilder()
                .WithDescription($"http://lmgtfy.com/?q={oh}")
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            await ReplyAsync("", false, embed);
        }

        [Command("Ping")]
        public async Task Ping()
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithDescription(
                Bot.Internal.Utilities.GetFormattedLocaleMsg("PingCommandText", Program._client.Latency));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));

            await ReplyAsync("", false, embed);
        }

        [Command("Google")]
        public async Task Google([Remainder] string search)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            search = search.Replace(' ', '+');
            var searchUrl = $"https://google.com/search?q={search}";
            var embed = new EmbedBuilder();
            embed.WithDescription(searchUrl);
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithThumbnailUrl(
                "https://upload.wikimedia.org/wikipedia/commons/thumb/5/53/Google_%22G%22_Logo.svg/2000px-Google_%22G%22_Logo.svg.png");

            await ReplyAsync("", false, embed);
        }

        [Command("Invite")]
        public async Task InviteUserToUseBot()
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder
            {
                Description = "Invite the bot [here](https://bot.discord.io/SIVA)"
            };
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));

            await ReplyAsync("", false, embed);
        }

        [Command("Pluto")]
        public async Task SendPartnerInfo()
        {
            var embed = new EmbedBuilder()
                .WithDescription(
                    "**What is PlutoBot?**\nPlutoBot is a bot that is currently under development but already has moderation commands, channel logs, a cleverbot module, and some fun commands. It is developed by <@345318328195350528>.")
                .AddField("Invite the bot", "https://discord.io/plutoBot")
                .AddField("Pluto Support Server", "https://discord.gg/qTNEgPD")
                .WithColor(new Color(0x195AC4))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await ReplyAsync("", false, embed);
        }

        [Command("User")]
        public async Task GetUserFromText([Remainder] string arg)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var user = Context.Guild.Users.FirstOrDefault(x => x.Username == arg);
            if (user == null)
            {
                var embedNull = new EmbedBuilder();
                embedNull.WithDescription("User doesn't exist in this server.")
                    .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                    .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
                await ReplyAsync("", false, embedNull);
                return;
            }

            var embedNotNull = new EmbedBuilder()
                .AddField("Username: ", $"{user.Username}#{user.Discriminator}")
                .AddField("Game: ", user.Game.ToString() ?? "Nothing")
                .AddField("Status: ", user.Status)
                .AddField("Account Created: ", user.CreatedAt)
                .AddField("User ID: ", user.Id)
                .AddField("Is Bot", user.IsBot)
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithThumbnailUrl(user.GetAvatarUrl());
            await ReplyAsync("", false, embedNotNull);
        }
    }
}