using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Commands.Modules;
using Volte.Helpers;
using Volte.Interactions;

namespace Volte.Commands.Application
{
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
                            .WithButtons(Buttons.Primary("text:nato:raw", "Raw"));
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        reply.WithEmbeds(ctx.CreateEmbedBuilder()
                            .WithTitle($"There is not a NATO word for the character {e.ParamName}")
                            .WithDescription("Only standard English letters and numbers are valid."));
                    }

                    break;
                case "zalgo":
                    var intensity = ((ZalgoIntensity)((sbyte)subcommand.GetOption("intensity").GetAsString().First()));
                    reply.WithContent(ZalgoHelper.GenerateZalgo(content, intensity, IncludeChars.All));
                    break;
            }

            await reply.RespondAsync();
        }

        public override async Task HandleComponentAsync(MessageComponentContext ctx)
        {
            await (ctx.Id.Action switch
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