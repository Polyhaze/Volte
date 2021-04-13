using System;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands;
using Volte.Core.Helpers;

namespace Volte.Interactive
{
    public class DeleteMessageReactionCallback : IReactionCallback
    {
        public RunMode RunMode { get; } = RunMode.Parallel;
        public ICriterion<SocketReaction> Criterion { get; } = new EnsureReactionFromSourceUserCriterion();
        public TimeSpan? Timeout { get; } = 10.Seconds();
        public VolteContext Context { get; }
        public RestUserMessage Message { get; private set; }
        public async Task<bool> HandleAsync(SocketReaction reaction)
        {
            if (reaction.Emote.Name.EqualsIgnoreCase(DiscordHelper.X))
            {
                return await reaction.Message.Value.TryDeleteAsync();
            }

            return false;

        }

        public DeleteMessageReactionCallback(VolteContext ctx, Embed embed)
        {
            _ = Executor.ExecuteAsync(async () => await (Message = await Context.Channel.SendMessageAsync(embed: embed)).AddReactionAsync(DiscordHelper.X.ToEmoji()));
            Context = ctx;
        }
    }
}