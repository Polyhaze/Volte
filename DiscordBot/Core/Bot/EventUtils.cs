using Discord;
using Discord.WebSocket;
using System.Linq;
using SIVA.Core.JsonFiles;
using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using System.IO;

namespace SIVA.Core.Bot
{
    internal class EventUtils
    {
        private static DiscordSocketClient _client;

        public static async Task Autorole(SocketGuildUser user)
        {
            var config = GuildConfig.GetGuildConfig(user.Guild.Id);
            if (config.Autorole != null || config.Autorole != "")
            {
                var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name == config.Autorole);
                await user.AddRoleAsync(targetRole);
            }
        }

        internal static async Task HandleMessages(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);
            if (msg == null) { Console.WriteLine($"{s} not cared for as it's null (for whatever reason)"); return; }
            //Console.WriteLine($"Var msg: {msg}");
            //Console.WriteLine($"Var s: {s}");
            //Console.WriteLine($"Var context: {context}");

            if (context.Guild == null)
            {
                var user = await context.User.GetOrCreateDMChannelAsync();
                await user.SendMessageAsync("Commands are not available in DMs.");
            }

            var config = GuildConfig.GetGuildConfig(context.Guild.Id) ?? GuildConfig.CreateGuildConfig(context.Guild.Id);
            config.GuildOwnerId = context.Guild.Owner.Id;
            GuildConfig.SaveGuildConfig();

            if (config.Leveling)
            {
                await Leveling.UserSentMessage((SocketGuildUser)context.User, (SocketTextChannel)context.Channel);
            }

            if (context.Guild.Id == 385902350432206849)
            {
                if (msg.Content.Contains("🎷") || msg.Content.Contains("🎺"))
                {
                    if (msg.Author.Id == 360493978371751937)
                    {
                        await msg.DeleteAsync();
                        var msgObj = await context.Channel.SendMessageAsync(context.User.Mention + " no");
                        Thread.Sleep(5000);
                        await msgObj.DeleteAsync();
                    }
                }
            }
        }

        public static async Task Goodbye(SocketGuildUser user)
        {
            var config = GuildConfig.GetGuildConfig(user.Guild.Id);

            if (config.WelcomeChannel != 0)
            {
                var a = config.LeavingMessage.Replace("{UserMention}", user.Mention);
                var b = a.Replace("{ServerName}", user.Guild.Name);
                var c = b.Replace("{UserName}", user.Username);
                var d = c.Replace("{OwnerMention}", user.Guild.Owner.Mention);
                var e = d.Replace("{UserTag}", user.DiscriminatorValue.ToString());

                var channel = user.Guild.GetTextChannel(config.WelcomeChannel);
                var embed = new EmbedBuilder();
                embed.WithDescription(e);
                embed.WithColor(new Color(config.WelcomeColour1, config.WelcomeColour2, config.WelcomeColour3));
                embed.WithFooter($"Guild Owner: {user.Guild.Owner.Username}#{user.Guild.Owner.Discriminator}");
                embed.WithThumbnailUrl(user.Guild.IconUrl);
                await channel.SendMessageAsync("", false, embed);

            }
        }

        public static async Task Welcome(SocketGuildUser user)
        {
            var config = GuildConfig.GetGuildConfig(user.Guild.Id);

            if (config.WelcomeChannel != 0)
            {
                var a = config.WelcomeMessage.Replace("{UserMention}", user.Mention);
                var b = a.Replace("{ServerName}", user.Guild.Name);
                var c = b.Replace("{UserName}", user.Username);
                var d = c.Replace("{OwnerMention}", user.Guild.Owner.Mention);
                var e = d.Replace("{UserTag}", user.DiscriminatorValue.ToString());

                var channel = user.Guild.GetTextChannel(config.WelcomeChannel);
                var embed = new EmbedBuilder();
                embed.WithDescription(e);
                embed.WithColor(new Color(config.WelcomeColour1, config.WelcomeColour2, config.WelcomeColour3));
                embed.WithThumbnailUrl(user.Guild.IconUrl);
                await channel.SendMessageAsync("", false, embed);
            }

            if (user.Guild.Id == 419612620090245140)
            {
                await user.ModifyAsync(x => 
                {
                    x.Nickname = $"{user.Username}.cs";
                });
            }
        }

        public static async Task GuildUtils(SocketGuild s)
        {

            var config = GuildConfig.GetGuildConfig(s.Id) ??
                         GuildConfig.CreateGuildConfig(s.Id);

            if (Config.bot.Blacklist.Contains(s.Owner.Id))
            {
                await s.LeaveAsync();
                return;
            }

            int Bots = 0;
            int Users = 0;
            foreach (SocketGuildUser user in s.Users)
            {
                if (user.IsBot)
                {
                    Bots += 1;
                }
                else
                {
                    Users += 1;
                }
            }

            if (Bots > Users)
            {
                var greemDm = await _client.GetUser(Config.bot.BotOwner).GetOrCreateDMChannelAsync();
                await greemDm.SendMessageAsync("", false, new EmbedBuilder().WithDescription($"Server {s.Name} is potentially harmful. They have {Bots} bots and {Users} users. Consider making the bot leave.").WithColor(Config.bot.DefaultEmbedColour));
            }

            var dmChannel = await s.Owner.GetOrCreateDMChannelAsync();
            var embed = new EmbedBuilder();
            embed.WithTitle($"Thanks for adding me to your server, {s.Owner.Username}!");
            embed.WithDescription("For quick information, visit the wiki: https://github.com/greemdotcs/greemdotcs.github.io/wiki \nNeed quick help? Visit the SIVA-dev server and create a support ticket: https://discord.gg/ubXaT6u \nTo get started, use the command `$h`. Follow that with a module to get a list of commands!");
            embed.WithThumbnailUrl(s.IconUrl);
            embed.WithFooter("Still need help? Visit the SIVA-dev server linked above.");
            embed.WithColor(Config.bot.DefaultEmbedColour);

            await dmChannel.SendMessageAsync("", false, embed);

            config.GuildOwnerId = s.Owner.Id;
            GuildConfig.SaveGuildConfig();

        }

        public static async Task AssholeChecks(SocketMessage s)
        {

            var msg = s as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;
            if (msg == null) return;

            var config = GuildConfig.GetGuildConfig(context.Guild.Id);

            try //attempt something
            {
                if (config.Antilink == false)
                {
                    //if antilink is turned off then do nothing.
                }
                else //if it isnt then do the following
                {
                    if (msg.Author.Id == config.GuildOwnerId) //if the message is from the guild owner
                    {
                        //don't do anything
                    }
                    else //if the message isnt from the guild owner, do the following
                    {
                        if ((msg.Content.Contains("https://discord.gg") || msg.Content.Contains("https://discord.io")) && !config.AntilinkIgnoredChannels.Contains(context.Channel.Id)) //if the message contains https://discord.gg or io (it's an invite link), then delete it
                        {
                            await msg.DeleteAsync();
                            var embed = new EmbedBuilder();
                            embed.WithDescription($"{context.User.Mention}, no invite links.");
                            embed.WithColor(Config.bot.DefaultEmbedColour);
                            var mssg = await context.Channel.SendMessageAsync("", false, embed);
                            Thread.Sleep(10000);
                            await mssg.DeleteAsync();
                        }
                    }
                }
            }
            catch (NullReferenceException) // if the config variable returns an invalid value then create the guild config
            {
                GuildConfig.CreateGuildConfig(context.Guild.Id);
            }

            if (config.MassPengChecks)
            {
                if (msg.Content.Contains("@everyone") || msg.Content.Contains("@here"))
                {
                    if (msg.Author != context.Guild.Owner)
                    {
                        await msg.DeleteAsync();
                        var msgg = await context.Channel.SendMessageAsync($"{msg.Author.Mention}, try not to mass ping.");
                        Thread.Sleep(4000);
                        await msgg.DeleteAsync();
                    }
                }
            }
        }

        internal static async Task Log(LogMessage msg)
        {
            if (!Config.bot.Debug) return;
            if (msg.Message.Contains("blocking the gateway task")) return;
            Console.WriteLine($"[{msg.Severity}]: ({msg.Source}): {msg.Message}");
            var msg2 = $"[{msg.Severity}]: ({msg.Source}): {msg.Message}";
            if (!msg2.Contains("(Rest)"))
            {
                var channel = Program._client.GetGuild(405806471578648588).GetTextChannel(431928769465548800).SendMessageAsync(msg2);
            }

            try
            {
                File.AppendAllText("Debug.log", $"[{msg.Severity}]: ({msg.Source}): {msg.Message}\n");
            }
            catch (FileNotFoundException)
            {
                File.WriteAllText("Debug.log", $"[{msg.Severity}]: ({msg.Source}): {msg.Message}\n");
            }
        }
    }
}
