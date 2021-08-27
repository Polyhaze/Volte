using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Volte.Commands.Slash
{
    public abstract class SlashCommand
    {
        public string Name { get; }
        public string Description { get; }

        protected SlashCommand(string name, string description)
        {
            Name = name.ToLower().Trim().Replace(" ", "-");
            Description = description;
        }

        //virtual to allow for ignoring the need to override this for commands that only require name and description.
        public virtual SlashCommandBuilder GetCommandBuilder(IServiceProvider provider) => new SlashCommandBuilder();

        public abstract Task HandleAsync(SlashCommandContext ctx);

        public virtual Task HandleComponentAsync(MessageComponentContext ctx) => Task.CompletedTask;
    }
}