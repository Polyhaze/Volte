using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

using Discord;
using Discord.WebSocket;

namespace BrackeysBot.Services
{
    public class PingLoggingService : BrackeysBotService, IInitializeableService
    {
        private DiscordSocketClient _discord;
        private DataService _data;

        public PingLoggingService(DiscordSocketClient discord, DataService data)
        {
            _discord = discord;
            _data = data;
        }

        public void Initialize()
        {
            _discord.MessageReceived += CheckMessageForPings;
        }

        private async Task CheckMessageForPings(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg) || !(msg.Channel is SocketGuildChannel channel) || msg.Author.IsBot)
                return;

            IEnumerable<IRole> roles = GetLoggableIDs().Select(id => channel.Guild.GetRole(id));
            IEnumerable<IRole> mentionedRoles = msg.MentionedRoles.Intersect(roles);

            if (mentionedRoles.Count() > 0)
            {
                await PostToLogChannel(msg, mentionedRoles, channel.Guild);
            }
        }

        public async Task PostToLogChannel(IMessage original, IEnumerable<IRole> mentionedRoles, IGuild guild)
        {
            ulong moderationLogChannelID = _data.Configuration.ModerationLogChannelID;
            if (moderationLogChannelID == 0)
                throw new Exception("Invalid moderation log channel ID.");

            ITextChannel channel = await guild.GetTextChannelAsync(moderationLogChannelID);

            string roleDisplay = string.Join(", ", mentionedRoles.Select(r => MentionUtils.MentionRole(r.Id).Envelop("**")));

            await channel.SendMessageAsync(string.Empty, false, new EmbedBuilder()
                .WithTitle("Role Mention")
                .WithColor(Color.Orange)
                .WithDescription($"{original.Author.Mention} mentioned {roleDisplay} in their [message]({original.GetJumpUrl()}).")
                .Build());
        }

        private IEnumerable<ulong> GetLoggableIDs()
            => _data.Configuration.LoggableIDs ?? new ulong[0];
    }
}
