using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Gommon;
using Volte.Core.Helpers;

namespace Volte.Services
{
    public sealed class StarboardService : VolteEventService
    {
        private readonly DatabaseService _db;
        private readonly DiscordShardedClient _client;

        public StarboardService(DatabaseService databaseService,
            DiscordShardedClient discordShardedClient)
        {
            _db = databaseService;
            _client = discordShardedClient;
        }


        public override Task DoAsync(EventArgs args)
        {
            return args switch
            {
                MessageReactionAddEventArgs reactionAdd => HandleReactionAddAsync(reactionAdd),
                MessageReactionRemoveEventArgs reactionRemove => HandleReactionRemoveAsync(reactionRemove),
                MessageReactionsClearEventArgs reactionsClear => HandleReactionsClearAsync(reactionsClear),
                _ => Task.CompletedTask
            };
        }

        private async Task HandleReactionAddAsync(MessageReactionAddEventArgs args)
        {
            if (args.Channel is DiscordDmChannel) return;
            if (args.Emoji.Name != EmojiHelper.Star) return;
            var data = _db.GetData(args.Guild.Id);
            var starboard = data.Configuration.Starboard;
            var c = await args.Client.GetChannelAsync(starboard.StarboardChannel);
            if (c is null) return;
            var entry = data.Extras.StarboardEntries.FirstOrDefault(x => x.MessageId == args.Message.Id);
            if (entry is not null)
            {
                
            }
        }

        private async Task HandleReactionRemoveAsync(MessageReactionRemoveEventArgs args)
        {
            
        }

        private async Task HandleReactionsClearAsync(MessageReactionsClearEventArgs args)
        {
            
        }

        public async Task PostToStarboardAsync(DiscordMessage message)
        {
            var data = _db.GetData(message.Channel.Guild);
            var c = _client.GetGuild(message.Channel.GuildId)
                .GetChannel(data.Configuration.Starboard.StarboardChannel);
            var stars = message.Reactions.Count(x => x.Emoji.Name.Equals(EmojiHelper.Star));
            if (data.Configuration.Starboard.StarsRequiredToPost > stars) return;
            if (c is not null)
            {
                var e = new DiscordEmbedBuilder()
                    .WithSuccessColor()
                    .WithDescription(message.Content)
                    .WithAuthor(message.Author)
                    .AddField("Original Message", message.JumpLink);

                await c.SendMessageAsync($"{EmojiHelper.Star} {stars}", embed: e.Build());
            }
        }
    }
}