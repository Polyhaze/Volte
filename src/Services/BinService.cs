using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Gommon;
using Volte.Core;
using Volte.Data.Models;
using Volte.Data.Models.Guild;

namespace Volte.Services
{
    [Service("Bin", "The main Service that handles HTTP POST requests for debug reports to bin.scarsz.me.")]
    public sealed class BinService
    {
        public static BinService Instance = VolteBot.GetRequiredService<BinService>();
        private DiscordSocketClient _client;
        private RestClient _http;

        public BinService(DiscordSocketClient discordSocketClient,
            RestClient restClient)
        {
            _client = discordSocketClient;
            _http = restClient;
        }

        public string Execute(GuildData config)
        {
            var key = RandomKey();
            var data = new BinData
            {
                Description = BinEncrypt($"Volte Debug Information for Support, generated {DateTimeOffset.Now.FormatDate()}.", key),
                Expiration = 120,
                Files = new List<BinFile>
                {
                    new BinFile
                    {
                        Name = BinEncrypt("GuildConfiguration.json", key),
                        Content = BinEncrypt(config.ToString(), key),
                        Description = BinEncrypt($"Guild Configuration for {_client.GetGuild(config.Id).Name}", key),
                        Type = BinEncrypt("text/plain", key)
                    }
                }
            };
            _http.BaseUrl = new Uri("https://bin.greemdev.net");
            var req = new RestRequest("v1/post", Method.POST);
            req.AddHeader("Content-Type", "application/json");
            req.RequestFormat = DataFormat.Json;
            req.Parameters.Clear();
            req.AddParameter("application/json", JsonConvert.SerializeObject(data), ParameterType.RequestBody);
            return $"{JsonConvert.DeserializeObject(_http.Execute(req).Content).Cast<JObject>().GetValue("bin")}#{key}";

        }

        private string BinEncrypt(string content, string key)
        {
            return Convert.ToBase64String(EncryptString(content, Encoding.ASCII.GetBytes(key)));
        }

        private byte[] EncryptString(string text, byte[] key)
        {
            byte[] enc;
            byte[] iv;

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();
                iv = aes.IV;

                aes.Mode = CipherMode.CBC;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(text);
                        }

                        enc = ms.ToArray();
                    }
                }
            }

            var combIv = new byte[iv.Length + enc.Length];
            Array.Copy(iv, 0, combIv, 0, iv.Length);
            Array.Copy(enc, 0, combIv, iv.Length, enc.Length);

            return combIv;
        }

        private string RandomKey()
        {
            var pool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var r = new Random();
            var chars = Enumerable.Range(0, 32)
                .Select(x => pool[r.Next(0, pool.Length - 1)]);
            return new string(chars.ToArray());
        }

    }
}