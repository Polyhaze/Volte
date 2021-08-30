using System;
using Discord;
using Volte.Interactions;

namespace Volte.Commands.Application
{
    public class ModerationCommand : ApplicationCommand
    {
        public ModerationCommand() : base("mod", "Volte's moderation command suite.", true) { }

        public override SlashCommandBuilder GetCommandSignature(IServiceProvider provider)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("ban")
                    .WithDescription("Ban someone.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("target")
                        .WithDescription("The target of your fury.")
                        .WithType(ApplicationCommandOptionType.String)
                        .WithRequired(true))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("delete-days")
                        .WithDescription("The amount of days of messages to delete.")
                        .WithType(ApplicationCommandOptionType.Integer)
                        .AddChoice("None", 0)
                        .AddChoice("1", 1)
                        .AddChoice("2", 2)
                        .AddChoice("3", 3)
                        .AddChoice("4", 4)
                        .AddChoice("5", 5)
                        .AddChoice("6", 6)
                        .AddChoice("7", 7)))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("ban-by-id")
                    .WithDescription("Ban someone by their Snowflake ID, allowing the ability to ban people who are not in your guild.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("target")
                        .WithDescription("The ID of the user to ban.")
                        .WithType(ApplicationCommandOptionType.Number)));
    }
}