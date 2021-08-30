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

        public static ApplicationCommandProperties[] GetCommandBuilders(this IEnumerable<ApplicationCommand> set, IServiceProvider provider = null)
        {
            return set.Select<ApplicationCommand, ApplicationCommandProperties>(x =>
            {
                return x.CommandType switch
                {
                    ApplicationCommandType.Slash => x.GetCommandSignature(provider)
                        .WithName(x.Name)
                        .WithDescription(x.Description)
                        .Build(),
                    ApplicationCommandType.Message => new MessageCommandBuilder().WithName(x.Name).Build(),
                    ApplicationCommandType.User => new UserCommandBuilder().WithName(x.Name).Build(),
                    _ => null
                };
            }).Where(x => x != null).ToArray();
        }


        public static SafeDictionary<string, object> GetOptionsWithValues(this SocketSlashCommandDataOption dataOpt)
            => dataOpt.GetOptions().ToDictionary(x => x.Key, x => x.Value.Value).AsSafe();

        public static T GetOptionOfValue<T>(this SocketSlashCommandDataOption dataOpt, string name)
            => dataOpt.GetOptionsWithValues()[name].Cast<T>();

        public static SafeDictionary<string, SocketSlashCommandDataOption> GetOptions(
            this SocketSlashCommandDataOption dataOpt)
            => dataOpt.Options is null
                ? new SafeDictionary<string, SocketSlashCommandDataOption>()
                : dataOpt.Options.ToDictionary(x => x.Name).AsSafe();

        public static SafeDictionary<string, SocketSlashCommandDataOption> GetOptions(
            this SocketSlashCommandData data)
            => data.Options is null
                ? new SafeDictionary<string, SocketSlashCommandDataOption>()
                : data.Options.ToDictionary(x => x.Name).AsSafe();

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