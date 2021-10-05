using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte;
using Volte.Entities;
using Volte.Helpers;
using Volte.Interactions;
using Volte.Interactive;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule
    {
        [Command("Say")]
        [Description("Bot repeats what you tell it to.")]
        public Task<ActionResult> SayAsync([Remainder, Description("What to say.")] string msg)
            => None(async () =>
            {
                await Context.CreateEmbed(msg).SendToAsync(Context.Channel);
                _ = await Context.Message.TryDeleteAsync();
            });

        [Command("SilentSay", "SSay")]
        [Description(
            "Runs the say command normally, but doesn't show the author in the message.")]
        public Task<ActionResult> SilentSayAsync([Remainder, Description("What to say.")] string msg)
            => None(async () =>
            {
                await new EmbedBuilder()
                    .WithColor(Config.SuccessColor)
                    .WithDescription(msg)
                    .SendToAsync(Context.Channel);
                _ = await Context.Message.TryDeleteAsync();
            });

        [Command("PlainSay", "PSay")]
        [Description("Bot repeats what you tell it to; outside of an embed.")]
        public Task<ActionResult> SayPlainAsync([Remainder, Description("What to say.")] string msg)
            => None(() => Context.Channel.SendMessageAsync(msg, allowedMentions: AllowedMentions.None));

        [Command("Reverse" /*, "esreveR"*/)]
        [Description("Bot replies with the argument value reversed.")]
        public Task<ActionResult> ReverseAsync([Remainder, Description("What to reverse.")] string content)
            => Ok(Format.Code(content.Reverse()));

        [Command("Zalgo")]
        [Description("Generate Zalgo text.")]
        [ShowUnixArgumentsInHelp(VolteUnixCommand.Zalgo)]
        public Task<ActionResult> ZalgoAsync([Description("The content to Zalgo-ify.")] string content,
            [Remainder, Description("The Unix-style arguments for options.")]
            Dictionary<string, string> options)
        {
            if (options.TryGetValue("max", out _))
                return Ok(ZalgoHelper.GenerateZalgo(content, ZalgoIntensity.High,
                    IncludeChars.Up | IncludeChars.Middle | IncludeChars.Down));

            var intensity = options.TryGetValue("intensity", out var result)
                ? result.ToLower() switch
                {
                    "high" => ZalgoIntensity.High,
                    "medium" => ZalgoIntensity.Medium,
                    "med" => ZalgoIntensity.Medium,
                    "low" => ZalgoIntensity.Low,
                    _ => ZalgoIntensity.Low
                }
                : ZalgoIntensity.Low;

            IncludeChars includeChars = 0;
            if (options.TryGetValue("up", out _))
                includeChars |= IncludeChars.Up;
            if (options.TryGetValue("mid", out _) || options.TryGetValue("middle", out _))
                includeChars |= IncludeChars.Middle;
            if (options.TryGetValue("down", out _))
                includeChars |= IncludeChars.Down;
            if (includeChars is 0)
                return BadRequest("No up/middle/down characters were allowed.");

            var zalgo = ZalgoHelper.GenerateZalgo(content, intensity, includeChars);
            if (zalgo.Length > 2000)
                return BadRequest("The result was too large to show in a Discord message.");

            return options.TryGetValue("plain", out _)
                ? Ok(async () =>
                {
                    await Context.Channel.SendMessageAsync(zalgo, allowedMentions: AllowedMentions.None);
                })
                : Ok(zalgo);
        }

        [Command("Nato")]
        [Description(
            "Translates a string into the NATO Phonetic Alphabet. If no string is provided, then a full rundown of the NATO alphabet is shown.")]
        public Task<ActionResult> NatoAsync([Remainder, Description("The text to \"translate.\"")] string input = null)
        {
            if (input.IsNullOrEmpty())
                return Ok(PaginatedMessage.NewBuilder()
                    .WithTitle("NATO Phonetic Alphabet")
                    .WithPages(_nato.Select(kvp => $"**{char.ToUpper(kvp.Key)}**: {Format.Code(kvp.Value)}"))
                    .SplitPages(12));

            // ReSharper disable once (Im) PossibleNullReferenceException
            //this legit cant happen because of the if statement above
            return Lambda.TryCatch<ActionResult, ArgumentOutOfRangeException>(() =>
                    Ok(Context.CreateEmbedBuilder()
                        .AddField("Input", Format.Code(input))
                        .AddField("Output", Format.Code(input.ToCharArray()
                            .Where(x => !char.IsWhiteSpace(x))
                            .Select(x => GetNato(char.ToLower(x)))
                            .Join(" "))
                        )),
                e => BadRequest(
                    $"There is not a NATO word for the character `{e.ParamName}`. Only standard English letters and numbers are valid."));
        }
    }

    public sealed class TextCommand : ApplicationCommand
    {
        public TextCommand() : base("text", "Set of commands for manipulating text.") => Signature(o =>
        {
            o.Subcommand("reverse", "Reverse the provided text content.", x =>
                x.RequiredString("content", "The text to reverse.")
            );
            o.Subcommand("nato", "Convert the provided text's characters into NATO phonetic alphabet.", x =>
                x.RequiredString("content", "The text to convert.")
            );
            o.Subcommand("zalgo", "Zalgo-ify the provided text.", x =>
            {
                x.RequiredString("content", "The text to zalgo.");
                x.OptionalString("intensity", "How intense do you want the zalgo to be? Default is High.",
                    choices: new Choices(
                        ("High", "h"),
                        ("Medium", "m"),
                        ("Low", "l")));
            });
        });

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var subcommand = ctx.Options.First().Value;
            var reply = ctx.CreateReplyBuilder(true);
            var content = subcommand.GetOption("content").GetAsString();

            switch (subcommand.Name)
            {
                case "reverse":
                    reply.WithEmbedFrom(Format.Code(content.Reverse()));
                    break;
                case "nato":
                    try
                    {
                        reply.WithEmbeds(ctx.CreateEmbedBuilder()
                                .AddField("Input", Format.Code(content))
                                .AddField("Output", Format.Code(content.ToCharArray()
                                    .Where(x => !char.IsWhiteSpace(x))
                                    .Select(UtilityModule.GetNato)
                                    .Join(" "))
                                ))
                            .WithButtons(ButtonBuilder.CreatePrimaryButton("Raw", "text:nato:raw"));
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        reply.WithEmbeds(ctx.CreateEmbedBuilder()
                            .WithTitle($"There is not a NATO word for the character {e.ParamName}")
                            .WithDescription("Only standard English letters and numbers are valid."));
                    }

                    break;
                case "zalgo":
                    break;
            }

            await reply.RespondAsync();
        }

        public override async Task HandleComponentAsync(MessageComponentContext ctx)
        {
            await (ctx.CustomIdParts[1] switch
            {
                "nato" => ctx.CreateReplyBuilder(true)
                    .WithContent(ctx.Message.Embeds.First().Fields
                        .First(x => x.Name is "Output")
                        .Value.Replace("`", string.Empty))
                    .RespondAsync(),
                _ => ctx.DeferAsync(true)
            });
        }
    }
}