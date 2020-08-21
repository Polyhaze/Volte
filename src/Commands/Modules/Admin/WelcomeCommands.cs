using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Group("Welcome", "W")]
        public sealed class WelcomeModule : VolteModule
        {
            public WelcomeService WelcomeService { get; set; }
            
            [Command("Channel", "C")]
            [Description("Sets the channel used for welcoming new users for this guild.")]
            [Remarks("welcome channel {Channel}")]
            public Task<ActionResult> WelcomeChannelAsync([Remainder]DiscordChannel channel)
            {
                ModifyData(data =>
                {
                    data.Configuration.Welcome.WelcomeChannel = channel.Id;
                    return data;
                });
                return Ok($"Set this guild's welcome channel to {channel.Mention}.");
            }

            [Command("Message", "Msg")]
            [Description(
                "Sets or shows the welcome message used to welcome new users for this guild.")]
            [Remarks("welcomemessage [String]")]
            public Task<ActionResult> WelcomeMessageAsync([Remainder]string message = null)
            {
                if (message is null)
                {
                    return Ok(new StringBuilder()
                        .AppendLine("The current welcome message for this guild is: ```")
                        .AppendLine(Context.GuildData.Configuration.Welcome.WelcomeMessage)
                        .Append("```")
                        .ToString());
                }

                ModifyData(data =>
                {
                    data.Configuration.Welcome.WelcomeMessage = message;
                    return data;
                });
                var welcomeChannel =
                    Context.Guild.GetChannel(Context.GuildData.Configuration.Welcome.WelcomeChannel);
                var sendingTest = welcomeChannel is null
                    ? "Not sending a test message as you do not have a welcome channel set." +
                      "Set a welcome channel to fully complete the setup!"
                    : $"Sending a test message to {welcomeChannel.Mention}. This message will have all formatting and placeholders replaced with an actual value.";

                return Ok(new StringBuilder()
                        .AppendLine($"Set this guild's welcome message to ```{message}```")
                        .AppendLine()
                        .AppendLine($"{sendingTest}").ToString(),
                    _ =>
                    {
                        if (welcomeChannel is not null)
                        {
                            return new DiscordEmbedBuilder()
                                .WithColor(Context.GuildData.Configuration.Welcome.WelcomeColor)
                                .WithDescription(
                                    Context.GuildData.Configuration.Welcome.FormatWelcomeMessage(Context.Member))
                                .WithThumbnail(Context.Member.AvatarUrl)
                                .WithCurrentTimestamp()
                                .SendToAsync(welcomeChannel);
                        }
                        return Task.CompletedTask;
                    });
            }

            [Command("Color", "Colour", "Cl")]
            [Description("Sets the color used for welcome embeds for this guild.")]
            [Remarks("welcome color {Color}")]
            public Task<ActionResult> WelcomeColorAsync([Remainder]DiscordColor color)
            {
                ModifyData(data =>
                {
                    data.Configuration.Welcome.WelcomeColor = color.Value;
                    return data;
                });
                return Ok("Successfully set this guild's welcome message embed color!");
            }

            [Command("LeavingMessage", "Lmsg")]
            [Description("Sets or shows the leaving message used to say bye for this guild.")]
            [Remarks("welcome leavingmessage [String]")]
            public Task<ActionResult> LeavingMessageAsync([Remainder]string message = null)
            {
                if (message is null)
                {
                    return Ok(new StringBuilder()
                        .AppendLine("The current leaving message for this guild is ```")
                        .AppendLine(Context.GuildData.Configuration.Welcome.LeavingMessage)
                        .Append("```")
                        .ToString());
                }

                ModifyData(data =>
                {
                    data.Configuration.Welcome.LeavingMessage = message;
                    return data;
                });
                var welcomeChannel =
                    Context.Guild.GetChannel(Context.GuildData.Configuration.Welcome.WelcomeChannel);
                var sendingTest = welcomeChannel is null
                    ? "Not sending a test message, as you do not have a welcome channel set. " +
                      "Set a welcome channel to fully complete the setup!"
                    : $"Sending a test message to {welcomeChannel.Mention}.";

                return Ok(new StringBuilder()
                        .AppendLine($"Set this server's leaving message to ```{message}```")
                        .AppendLine()
                        .AppendLine($"{sendingTest}")
                        .ToString(),
                    _ =>
                    {
                        if (welcomeChannel is not null)
                        {
                            return new DiscordEmbedBuilder()
                                .WithColor(Context.GuildData.Configuration.Welcome.WelcomeColor)
                                .WithDescription(
                                    Context.GuildData.Configuration.Welcome.FormatWelcomeMessage(Context.Member))
                                .WithThumbnail(Context.Member.AvatarUrl)
                                .WithCurrentTimestamp()
                                .SendToAsync(welcomeChannel);
                        }
                        return Task.CompletedTask;
                    });
            }

            [Command("DmMessage", "Dmm")]
            [Description("Sets the message to be (attempted to) sent to members upon joining.")]
            [Remarks("welcome dmmessage [String]")]
            public Task<ActionResult> WelcomeDmMessageAsync(string message = null)
            {
                if (message is null)
                {
                    return Ok(
                        $"Unset the WelcomeDmMessage that was previously set to: {Formatter.InlineCode(Context.GuildData.Configuration.Welcome.WelcomeDmMessage)}");
                }

                ModifyData(data =>
                {
                    data.Configuration.Welcome.WelcomeDmMessage = message;
                    return data;
                });
                return Ok($"Set the WelcomeDmMessage to: ```{message}```\n\nAttempting to send a test message.",
                    _ =>
                    {
                        try
                        {
                            return new DiscordEmbedBuilder()
                                .WithColor(Context.GuildData.Configuration.Welcome.WelcomeColor)
                                .WithDescription(
                                    Context.GuildData.Configuration.Welcome.FormatWelcomeMessage(Context.Member))
                                .WithThumbnail(Context.Member.AvatarUrl)
                                .WithCurrentTimestamp()
                                .SendToAsync(Context.Member);
                        }
                        catch (Exception)
                        {
                            return Task.CompletedTask;
                        }
                        
                    });
            }
        }
    }
}