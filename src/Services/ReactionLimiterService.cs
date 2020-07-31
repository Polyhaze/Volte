using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace BrackeysBot.Services
{
    public class ReactionLimiterService : BrackeysBotService, IInitializeableService
    {
        private readonly DiscordSocketClient _discord;
        private readonly DataService _dataService;

        public ReactionLimiterService(
            DiscordSocketClient discord,
            DataService dataService)
        {
            _discord = discord;
            _dataService = dataService;
        }
        
        public void Initialize()
        {
            _discord.ReactionAdded += CheckReactionAsync;
        }

        public async Task CheckReactionAsync(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel channel, SocketReaction reaction) 
        {
            Dictionary<ulong, List<string>> restricted = _dataService.Configuration.EmoteRestrictions;

            if (restricted != null) 
            {
                List<string> limitedToEmotes = restricted.GetValueOrDefault(channel.Id);

                if (limitedToEmotes != null && limitedToEmotes.Count > 0 && !limitedToEmotes.Contains(reaction.Emote.StringVal())) 
                {
                    ulong messageId = reaction.MessageId;
                    IUserMessage message = await channel.GetMessageAsync(messageId) as IUserMessage;

                    await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                }
            }
        }
    }
}