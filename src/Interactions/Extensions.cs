using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Volte;
using Volte.Entities;
using Volte.Helpers;

namespace Volte.Interactions
{
    public static class Extensions
    {
        public static SelectMenuBuilder AddOptions(this SelectMenuBuilder menu,
            IEnumerable<SelectMenuOptionBuilder> options)
        {
            options.ForEach(opt => menu.AddOption(opt));
            return menu;
        }

        public static ApplicationCommandProperties[] GetCommandBuilders(this IEnumerable<ApplicationCommand> set,
            IServiceProvider provider = null)
            => set.Select<ApplicationCommand, ApplicationCommandProperties>(x => x.CommandType switch
                {
                    ApplicationCommandType.Slash => x.Sig.Apply(s =>
                    {
                        s.Builder.WithName(x.Name);
                        s.Builder.WithDescription(x.Description);
                    }),
                    ApplicationCommandType.Message => new MessageCommandBuilder().WithName(x.Name).Build(),
                    ApplicationCommandType.User => new UserCommandBuilder().WithName(x.Name).Build(),
                    _ => null
                }
            ).Where(x => x != null).ToArray();

        public static ActionRowBuilder AddComponentIf(this ActionRowBuilder builder, bool condition, IMessageComponent component)
        {
            if (condition)
            {
                if (builder.Components.Count >= 5)
                    throw new InvalidOperationException("Cannot have more than 5 components in a single Action Row.");
                
                builder.Components.Add(component);
            }
            return builder;
        }

        public static ApplicationCommandOptionData GetOption(this SocketSlashCommandDataOption dataOpt, string name)
            => dataOpt.GetOptions()[name];

        public static SafeDictionary<string, ApplicationCommandOptionData> GetOptions(
            this SocketSlashCommandDataOption dataOpt)
            => dataOpt.Options is null
                ? new SafeDictionary<string, ApplicationCommandOptionData>()
                : dataOpt.Options.ToDictionary(x => x.Name, x => new ApplicationCommandOptionData(x)).AsSafe();

        public static SafeDictionary<string, ApplicationCommandOptionData> GetOptions(
            this SocketSlashCommandData data)
            => data.Options is null
                ? new SafeDictionary<string, ApplicationCommandOptionData>()
                : data.Options.ToDictionary(x => x.Name, x => new ApplicationCommandOptionData(x)).AsSafe();

        public static ActionRowBuilder AsActionRow(this IEnumerable<IMessageComponent> components)
            => new ActionRowBuilder().AddComponents(components);

        public static ActionRowBuilder AddComponents(this ActionRowBuilder builder,
            IEnumerable<IMessageComponent> components)
            => builder.Apply(x => components.ForEach(c => x.AddComponent(c)));

        public static ComponentBuilder AddActionRow(this ComponentBuilder builder,
            Action<ActionRowBuilder> initializer)
            => builder.AddActionRows(new ActionRowBuilder().Apply(initializer));

        public static ComponentBuilder AddActionRows(this ComponentBuilder builder,
            IEnumerable<ActionRowBuilder> actionRows) => builder.AddActionRows(actionRows.ToArray());

        public static ComponentBuilder AddActionRows(this ComponentBuilder builder,
            params ActionRowBuilder[] actionRows)
        {
            builder.ActionRows ??= new List<ActionRowBuilder>();
            builder.ActionRows.AddRange(actionRows);
            return builder;
        }

        public static EmbedBuilder CreateEmbedBuilder(this SocketSlashCommand command, string content = null)
            => new EmbedBuilder().WithDescription(content ?? string.Empty)
                .WithColor(command.User.Cast<SocketGuildUser>()?.GetHighestRole()?.Color ?? Config.SuccessColor);

        public static SocketSlashCommandDataOption GetOption(this SocketSlashCommand command, string name) =>
            command.Data.Options?.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));

        public static T GetValueOr<T>(this SocketSlashCommandDataOption option, object @default) =>
            (option?.Value ?? @default).Cast<T>();
    }
}