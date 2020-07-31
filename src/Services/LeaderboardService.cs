using System.Linq;
using System.Collections.Generic;

using Discord;
using Discord.WebSocket;

namespace BrackeysBot.Services
{
    public class LeaderboardService : BrackeysBotService
    {
        public struct LeaderboardEntry
        {
            public IUser User { get; set; }
            public int Points { get; set; }
        }

        private readonly DataService _data;
        private readonly DiscordSocketClient _client;

        public LeaderboardService(DataService data, DiscordSocketClient client)
        {
            _data = data;
            _client = client;
        }

        public int GetUserPoints(IUser user)
        {
            if (_data.UserData.TryGetUser(user.Id, out UserData data))
                return data.Points;
            else
                return 0;
        }
        public void SetUserPoints(IUser user, int amount)
        {
            UserData userData = _data.UserData.GetOrCreate(user.Id);
            userData.Points = amount;

            _data.SaveUserData();
        }

        public LeaderboardEntry[] GetLeaderboard(int top = 10)
            => FilterValidLeaderboardUsers()
                .OrderByDescending(u => u.Points)
                .Take(top)
                .Select(u => new LeaderboardEntry { User = _client.GetUser(u.ID), Points = u.Points })
                .ToArray();

        public int GetUserRank(IUser user)
            => GetUserRank(user, FilterValidLeaderboardUsers());
        private int GetUserRank(IUser user, IEnumerable<UserData> ofUsers)
            => ofUsers.OrderByDescending(u => u.Points).ToList().FindIndex(u => u.ID == user.Id) + 1;
        public string FormatRank(IUser user)
        {
            var users = FilterValidLeaderboardUsers();
            int rank = GetUserRank(user, users);
            return rank == 0 ? string.Empty : $"Rank {rank} of {users.Count()}.";
        }

        private IEnumerable<UserData> FilterValidLeaderboardUsers()
            => _data.UserData.Users.Where(u => u.Points > 0 && _client.GetUser(u.ID) != null);
    }
}
