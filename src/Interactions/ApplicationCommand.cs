using System;
using System.Threading.Tasks;
using Discord;

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
        public string Name { get; }
        
        /// <summary>
        ///     The description of this <see cref="ApplicationCommand"/>, for use in Slash commands.<br/>
        ///     This property's value will be null if <see cref="CommandType"/> is anything but <see cref="ApplicationCommandType.Slash"/>.
        /// </summary>
        public string Description { get; }
        
        public bool IsLockedToGuild { get; }

        /// <summary>
        ///     Base constructor for non-Slash commands.
        /// </summary>
        /// <param name="name">The command name. Can contain spaces and caps.</param>
        /// <param name="commandType">The type of command, either Message or User. Use the other constructor for Slash commands.</param>
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
        protected ApplicationCommand(string name, string description, bool guildOnly = false)
        {
            Name = name.Trim().Replace(" ", "-").ToLower();
            Description = description;
            IsLockedToGuild = guildOnly;
        }

        //virtual to allow for ignoring the need to override this for commands that only require name and description.
        public virtual SlashCommandBuilder GetCommandSignature(IServiceProvider provider) => new SlashCommandBuilder();

        public virtual Task HandleSlashCommandAsync(SlashCommandContext ctx)
            => Task.CompletedTask;

        public virtual Task HandleUserCommandAsync(UserCommandContext ctx)
            => Task.CompletedTask;

        public virtual Task HandleMessageCommandAsync(MessageCommandContext ctx) 
            => Task.CompletedTask;

        public virtual Task HandleComponentAsync(MessageComponentContext ctx) => Task.CompletedTask;
    }
}