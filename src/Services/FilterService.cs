using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace BrackeysBot.Services
{
    public class FilterService : BrackeysBotService, IInitializeableService
    {
        private readonly DiscordSocketClient _discord;
        private readonly DataService _dataService;
        private readonly ModerationService _moderationService;
        private readonly ModerationLogService _loggingService;

        public FilterService(
            DiscordSocketClient discord,
            DataService dataService,
            ModerationService moderationService,
            ModerationLogService loggingService)
        {
            _discord = discord;
            _dataService = dataService;
            _moderationService = moderationService;
            _loggingService = loggingService;
        }
        public void Initialize()
        {
            _discord.MessageReceived += CheckMessageAsync;
            _discord.MessageUpdated += CheckEditedMessageAsync;
        }

        public async Task CheckMessageAsync(SocketMessage s) 
        {
            if (!(s is SocketUserMessage msg) || CanUseFilteredWords(msg))
                return;

            string content = msg.Content;
            
            if (ContainsBlockedWord(content)) 
                await DeleteMsgAndInfractUser(s as SocketUserMessage, content);
        }
        public async Task CheckEditedMessageAsync(Cacheable<IMessage, ulong> cacheable, SocketMessage s, ISocketMessageChannel channel)
        {
            if (!s.EditedTimestamp.HasValue)
                return;

            await CheckMessageAsync(s);
        }

        private bool ContainsBlockedWord(string msg) 
        {
            string[] blockedWords = _dataService.Configuration.BlockedWords;

            if (blockedWords == null)
                return false;

            return blockedWords.Any(str => new Regex($".*{str}.*").IsMatch(msg.ToLowerInvariant()));
        }

        private bool CanUseFilteredWords(SocketUserMessage msg)
        {
            return msg.Author.IsBot || (msg.Author as IGuildUser).GetPermissionLevel(_dataService.Configuration) >= PermissionLevel.Moderator;
        }

        private async Task DeleteMsgAndInfractUser(SocketUserMessage s, string message)
        {
            SocketGuildUser target = s.Author as SocketGuildUser;

            IReadOnlyCollection<SocketMessage> nearbyMessages = s.Channel.GetCachedMessages(s, Direction.Before, 1);
            SocketUserMessage linkMessage = nearbyMessages.Any() ? nearbyMessages.Last() as SocketUserMessage : s;
            string url = linkMessage.GetJumpUrl();

            // Delete message before creating infractions and notifying moderators because
            //  if something fails during infraction creation or notifying the moderators we 
            //  are at least certain the message got deleted.
            await s.DeleteAsync();

            _moderationService.AddInfraction(target, 
                    Infraction.Create(_moderationService.RequestInfractionID())
                    .WithType(InfractionType.Warning)
                    .WithModerator(_discord.CurrentUser)
                    .WithAdditionalInfo($"[Go near message]({url})\n**{message}**")
                    .WithDescription("Used filtered word"));

            await _loggingService.CreateEntry(ModerationLogEntry.New
                    .WithActionType(ModerationActionType.Filtered)
                    .WithTarget(target)
                    .WithReason($"[Go near message]({url})\n**{message}**")
                    .WithTime(DateTimeOffset.Now)
                    .WithModerator(_discord.CurrentUser));

            NotifyUser(s);
        }

        private async void NotifyUser(SocketMessage s)
        {
            IMessage msg = (await s.Channel.SendMessageAsync($"Hey {s.Author.Id.Mention()}! Your message goes against Discord COC, if you believe this is an error contact a Staff member!")) as IMessage;
            msg.TimedDeletion(5000);
        }
    }
}