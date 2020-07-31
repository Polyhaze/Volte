using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Discord;

using Humanizer;
using BrackeysBot.Core.Models;

namespace BrackeysBot.Services
{
    public class ModerationService : BrackeysBotService
    {
        private readonly DataService _data;

        public ModerationService(DataService data)
        {
            _data = data;
        }

        public void AddInfraction(IUser user, Infraction infraction)
        {
            var userData = _data.UserData.GetOrCreate(user.Id);
            userData.Infractions.Add(infraction);

            _data.SaveUserData();

            SendInfractionMessageToUser(user, infraction);
        }

        public void AddTemporaryInfraction(TemporaryInfractionType type, IUser user, IUser moderator, TimeSpan duration, string reason = "")
        {
            Infraction infraction = AddTemporaryInfraction(type, user.Id, moderator, duration, reason);
            SendInfractionMessageToUser(user, infraction);
        }
        public Infraction AddTemporaryInfraction(TemporaryInfractionType type, ulong userId, IUser moderator, TimeSpan duration, string reason = "")
        {
            var userData = _data.UserData.GetOrCreate(userId);

            // Ensure that same-type infractions do not stack.
            userData.TemporaryInfractions.RemoveAll(i => i.Type == type);

            userData.TemporaryInfractions.Add(TemporaryInfraction.Create(type, DateTime.UtcNow.Add(duration)));
            Infraction infraction = Infraction.Create(RequestInfractionID())
                .WithType(type.AsInfractionType())
                .WithModerator(moderator)
                .WithDescription(reason)
                .WithAdditionalInfo($"Duration: {duration.Humanize(7)}");

            userData.Infractions.Add(infraction);

            _data.SaveUserData();

            return infraction;
        }
        public void ClearTemporaryInfraction(TemporaryInfractionType type, IUser user)
            => ClearTemporaryInfraction(type, user.Id);
        public void ClearTemporaryInfraction(TemporaryInfractionType type, ulong userId)
        {
            var userData = _data.UserData.GetOrCreate(userId);
            userData.TemporaryInfractions.RemoveAll(i => i.Type == type);

            _data.SaveUserData();
        }

        public int ClearInfractions(IUser user)
        {
            if (_data.UserData.HasUser(user.Id))
            {
                UserData userData = _data.UserData.GetUser(user.Id);
                int infractionCount = userData.Infractions.Count;
                userData.Infractions.Clear();

                _data.SaveUserData();

                return infractionCount;
            }
            return 0;
        }
        public bool DeleteInfraction(int id)
        {
            if (TryGetInfraction(id, out Infraction _, out ulong userId))
            {
                _data.UserData.GetUser(userId).Infractions.RemoveAll(i => i.ID == id);
                _data.SaveUserData();
                return true;
            }
            return false;
        }

        public bool TryGetInfraction(int id, out Infraction infraction, out ulong userId)
        {
            UserData data = _data.UserData.Users.FirstOrDefault(u => u.Infractions.Any(i => i.ID == id));

            infraction = data?.Infractions.FirstOrDefault(i => i.ID == id) ?? default;
            userId = data?.ID ?? 0;

            return data != null;
        }

        public int RequestInfractionID()
            => _data.UserData.Users.Count > 0
                ? 1 + _data.UserData.Users.Max(u => u.Infractions?.Count > 0 ? u.Infractions.Max(i => i.ID) : -1)
                : 1;

    
        private async void SendInfractionMessageToUser(IUser user, Infraction infraction) 
        {
            // Tempban and Ban send a DM themselves, we don't have to send a duplicate.
            if (infraction.Type == InfractionType.TemporaryBan || infraction.Type == InfractionType.Ban)
                return;

            UserData userData = _data.UserData.GetUser(user.Id);
            int infractionCount = userData.Infractions.Count;
            string message = $"Hey there! You were **{GetInfractionTypeString(infraction.Type)}** for **{infraction.Description}**! You currently have **{infractionCount}** infraction(s). Be careful; accumulating infractions may result in restricted access or even (permanent) removal from the server!";

            await user.TrySendMessageAsync(message);
        }

        private string GetInfractionTypeString(InfractionType type) 
        {
            switch (type) 
            {
                case InfractionType.Kick:
                    return "Kicked";
                case InfractionType.Mute:
                    return "Muted";
                case InfractionType.Warning:
                    return "Warned";
                case InfractionType.TemporaryMute:
                    return "Temporarily Muted";
                default:
                    return "Given an Infraction";  
            }
        }
    }
}
