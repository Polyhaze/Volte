using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;

using YamlDotNet.Serialization;

namespace BrackeysBot.Services
{
    public class DataService : BrackeysBotService
    {
        public BotConfiguration Configuration => _configuration;
        public UserDataCollection UserData => _userData;

        private BotConfiguration _configuration;
        private UserDataCollection _userData;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = true
        };

        private const string _databasePath = "users.json";
        private const string _configPath = "config.yaml";

        public DataService()
        {
            LoadConfiguration();
            LoadUserData();
        }

        public void SaveConfiguration()
        {
            var serializer = new SerializerBuilder().ConfigureDefaultValuesHandling(DefaultValuesHandling.Preserve).Build();
            string yaml = serializer.Serialize(_configuration);
            File.WriteAllText(_configPath, yaml);
        }
        public void LoadConfiguration()
        {
            if (!File.Exists(_configPath))
            {
                _configuration = new BotConfiguration();
                SaveConfiguration();
                return;
            }

            var deserializer = new DeserializerBuilder().Build();
            _configuration = deserializer.Deserialize<BotConfiguration>(File.ReadAllText(_configPath));
        }

        public void SaveUserData()
        {
            string json = JsonSerializer.Serialize(_userData, _jsonOptions);
            File.WriteAllText(_databasePath, json);
        }
        public void LoadUserData()
        {
            if (!File.Exists(_databasePath))
            {
                _userData = new UserDataCollection();
                SaveUserData();
                return;
            }

            string json = File.ReadAllText(_databasePath);
            _userData = JsonSerializer.Deserialize<UserDataCollection>(json, _jsonOptions);
        }
    }
}
