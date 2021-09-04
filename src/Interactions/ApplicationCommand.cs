using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;

namespace Volte.Interactions
{
    /// <summary>
    ///     The base class for Volte's interaction commands, along with handlers for their message components.
    /// </summary>
    public abstract class ApplicationCommand
    {
        public ApplicationCommandType CommandType { get; } = ApplicationCommandType.Slash;

        /// <summary>
        ///     The name of this <see cref="ApplicationCommand"/> this class represents.<br/><br/>
        /// 
        ///     When <see cref="CommandType"/> is <see cref="ApplicationCommandType.Slash"/>,
        ///     this property's value will have spaces replaced with hyphens and be all lowercase.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     The description of this <see cref="ApplicationCommand"/>, for use in Slash commands.<br/>
        ///     This property's value will be null if <see cref="CommandType"/> is anything but <see cref="ApplicationCommandType.Slash"/>.
        /// </summary>
        public string Description { get; protected set; }

        public bool IsLockedToGuild { get; }

        public virtual Task<bool> RunSlashChecksAsync(SlashCommandContext ctx) => Task.FromResult(true);
        public virtual Task<bool> RunUserChecksAsync(UserCommandContext ctx) => Task.FromResult(true);
        public virtual Task<bool> RunMessageChecksAsync(MessageCommandContext ctx) => Task.FromResult(true);
        public virtual Task<bool> RunMessageComponentChecksAsync(MessageComponentContext ctx) => Task.FromResult(true);

        /// <summary>
        ///     Base constructor for non-Slash commands.
        /// </summary>
        /// <param name="name">The command name. Can contain spaces and caps.</param>
        /// <param name="commandType">The type of command, either Message or User. Use the other constructor for Slash commands.</param>
        /// <param name="guildOnly">Whether or not to restrict this command to guild-only.</param>
        protected ApplicationCommand(string name, ApplicationCommandType commandType, bool guildOnly = false)
        {
            CommandType = commandType;
            Name = name;
            Description = null;
            IsLockedToGuild = guildOnly;
        }

        /// <summary>
        ///     Base constructor for Slash commands.
        /// </summary>
        /// <param name="name">The command name. May not contain spaces and caps.</param>
        /// <param name="description">The Slash command's description.</param>
        /// <param name="guildOnly">Whether or not to restrict this command to guild-only.</param>
        protected ApplicationCommand(string name, string description, bool guildOnly = false)
        {
            Name = name.Trim().Replace(" ", "-").ToLower();
            Description = description;
            IsLockedToGuild = guildOnly;
        }

        //virtual to allow for ignoring the need to override this for commands that only require name and description.
        public virtual SlashCommandSignature GetSignature(IServiceProvider provider) => SlashCommandSignature.Command();

        public virtual Task HandleSlashCommandAsync(SlashCommandContext ctx)
            => Task.CompletedTask;

        public virtual Task HandleUserCommandAsync(UserCommandContext ctx)
            => Task.CompletedTask;

        public virtual Task HandleMessageCommandAsync(MessageCommandContext ctx)
            => Task.CompletedTask;

        public virtual Task HandleComponentAsync(MessageComponentContext ctx) => Task.CompletedTask;
    }

    public sealed class ApplicationCommandOptionData
    {
        public ApplicationCommandOptionData(SocketSlashCommandDataOption option)
            => BackingData = option;

        public string Name => BackingData.Name;
        public ApplicationCommandOptionType Type => BackingData.Type;
        public object RawValue => BackingData.Value;
        public SocketSlashCommandDataOption BackingData { get; }

        public IEnumerable<ApplicationCommandOptionData> Options
            => BackingData.Options?.Select(x => new ApplicationCommandOptionData(x))
               ?? Array.Empty<ApplicationCommandOptionData>();

        public ApplicationCommandOptionData GetOption(string name) =>
            Options.FirstOrDefault(x => x.BackingData.Name.EqualsIgnoreCase(name));

        public string GetAsString() => 
            BackingData.Value?.ToString();

        public double GetAsDouble() =>
            double.TryParse(GetAsString(), out var result)
                ? result
                : double.MaxValue;

        public ulong GetAsLong() =>
            ulong.TryParse(GetAsString(), out var result)
                ? result
                : ulong.MaxValue;

        public int GetAsInteger() =>
            int.TryParse(GetAsString(), out var result)
                ? result
                : int.MaxValue;

        public bool GetAsBoolean() =>
            BackingData.Value.HardCast<bool>();

        public SocketRole GetAsRole(SocketRole defaultValue = null) =>
            BackingData.Value.Cast<SocketRole>() ?? defaultValue;

        public SocketUser GetAsUser(SocketUser defaultValue = null) =>
            BackingData.Value.Cast<SocketUser>() ?? defaultValue;

        public SocketGuildUser GetAsGuildUser(SocketGuildUser defaultValue = null) =>
            GetAsUser().Cast<SocketGuildUser>() ?? defaultValue;

        public SocketGuildChannel GetAsGuildChannel(SocketGuildChannel defaultValue = null) =>
            BackingData.Value.Cast<SocketGuildChannel>() ?? defaultValue;

        public SocketTextChannel GetAsTextChannel(SocketTextChannel defaultValue = null) =>
            GetAsGuildChannel().Cast<SocketTextChannel>() ?? defaultValue;

        public SocketVoiceChannel GetAsVoiceChannel(SocketVoiceChannel defaultValue = null) =>
            GetAsGuildChannel().Cast<SocketVoiceChannel>() ?? defaultValue;

        public SocketCategoryChannel GetAsCategory(SocketCategoryChannel defaultValue = null) =>
            GetAsGuildChannel().Cast<SocketCategoryChannel>() ?? defaultValue;
    }
}