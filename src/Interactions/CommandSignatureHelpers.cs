using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Discord;
using Gommon;

namespace Volte.Interactions
{
    public class SlashCommandSignature
    {
        public static SlashCommandSignature Command(Action<Options> optionsProducer)
            => new SlashCommandSignature(new SlashCommandBuilder()).Options(optionsProducer);

        public static SlashCommandSignature Command()
            => new SlashCommandSignature(new SlashCommandBuilder());

        public static SlashCommandSignature Subcommand(string name, string description, bool isGroup = false)
            => new SlashCommandSignature(new SlashCommandOptionBuilder().WithName(name)
                .WithDescription(description)
                .WithType(isGroup
                    ? ApplicationCommandOptionType.SubCommandGroup
                    : ApplicationCommandOptionType.SubCommand));

        public SlashCommandBuilder Builder { get; }
        public SlashCommandOptionBuilder SubcommandBuilder { get; }

        private SlashCommandSignature(SlashCommandOptionBuilder builder)
        {
            SubcommandBuilder = builder;
        }

        private SlashCommandSignature(SlashCommandBuilder builder)
        {
            Builder = builder;
        }

        public static implicit operator SlashCommandBuilder(SlashCommandSignature signature)
            => signature.Builder;

        public static implicit operator ApplicationCommandProperties(SlashCommandSignature signature)
            => signature.Builder.Build();

        public static implicit operator ApplicationCommandOptionProperties(SlashCommandSignature signature)
            => signature.SubcommandBuilder.Build();


        public SlashCommandSignature Options(Action<Options> optionsProducer)
        {
            var opts = new Options();
            optionsProducer(opts);

            if (Builder is null)
                opts.Builders.ForEach(x => SubcommandBuilder.AddOption(x));
            else if (!opts.Builders.IsEmpty())
                Builder.AddOptions(opts.Builders.ToArray());

            return this;
        }
    }

    public class Choices
    {
        public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();

        public Choices(params (string Label, object Value)[] choices)
            => choices.ForEach(c => Values.Add(c.Label, c.Value));
    }

    public class Options
    {
        public List<SlashCommandOptionBuilder> Builders { get; } = new List<SlashCommandOptionBuilder>();

        private void Add(
            ApplicationCommandOptionType type,
            string name,
            string description,
            bool? required = null,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Action<Options> optionsProducer = null,
            Choices choices = null)
        {
            var opt = new SlashCommandOptionBuilder()
                .WithName(name)
                .WithDescription(description)
                .WithType(type);

            if (required.HasValue)
                opt.WithRequired(required.Value);
            
            // ReSharper disable once UseDeconstructionOnParameter
            // it's legit just a new variable declaration. no
            choices?.Values?.ForEach(c =>
            {
                switch (c.Value)
                {
                    case int integer:
                        opt.AddChoice(c.Key, integer);
                        break;
                    default:
                        opt.AddChoice(c.Key, c.Value.ToString());
                        break;
                }
            });


            var options = new Options();
            optionsProducer?.Invoke(options);
            options.Builders.ForEach(x => opt.AddOption(x));

            builderModifier?.Invoke(opt);

            Builders.Add(opt);
        }

        public void SubcommandGroup(string name, string description, Action<Options> optionsProducer = null)
            => Add(ApplicationCommandOptionType.SubCommandGroup, name, description, optionsProducer: optionsProducer);

        public void Subcommand(string name, string description, Action<Options> optionsProducer = null)
            => Add(ApplicationCommandOptionType.SubCommand, name, description, optionsProducer: optionsProducer);

        public void Subcommand(SlashCommandSignature signature)
            => Add(signature.SubcommandBuilder.Type, signature.SubcommandBuilder.Name,
                signature.SubcommandBuilder.Description, true,
                b => signature.SubcommandBuilder.Options.ForEach(x => b.AddOption(x)));

        public void RequiredString(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.String, name, description, true, builderModifier, choices: choices);

        public void OptionalString(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.String, name, description, false, builderModifier, choices: choices);

        public void RequiredBoolean(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Boolean, name, description, true, builderModifier, choices: choices);

        public void OptionalBoolean(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Boolean, name, description, false, builderModifier, choices: choices);

        public void RequiredChannel(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Channel, name, description, true, builderModifier, choices: choices);

        public void OptionalChannel(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Channel, name, description, false, builderModifier, choices: choices);

        public void RequiredInteger(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Integer, name, description, true, builderModifier, choices: choices);

        public void OptionalInteger(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Integer, name, description, false, builderModifier, choices: choices);

        public void RequiredMentionable(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Mentionable, name, description, true, builderModifier, choices: choices);

        public void OptionalMentionable(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Mentionable, name, description, false, builderModifier, choices: choices);

        public void RequiredDouble(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Number, name, description, true, builderModifier, choices: choices);

        public void OptionalDouble(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Number, name, description, false, builderModifier, choices: choices);

        public void RequiredRole(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Role, name, description, true, builderModifier, choices: choices);

        public void OptionalRole(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.Role, name, description, false, builderModifier, choices: choices);

        public void RequiredUser(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.User, name, description, true, builderModifier, choices: choices);

        public void OptionalUser(string name, string description,
            Action<SlashCommandOptionBuilder> builderModifier = null,
            Choices choices = null)
            => Add(ApplicationCommandOptionType.User, name, description, false, builderModifier, choices: choices);
    }
}