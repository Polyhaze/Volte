using System.IO;
using Newtonsoft.Json;

namespace Volte.Dashboard {
    public class AspConfig {
        private static readonly WebConfig Web;
        private const string ConfigFile = "data/webconfig.json";

        static AspConfig() {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            if (!File.Exists(ConfigFile)) {
                Web = new WebConfig {
                    AppId = "",
                    AppSecret = "",
                };
                var json = JsonConvert.SerializeObject(Web, Formatting.Indented);
                File.WriteAllText(ConfigFile, json);
            }
            else {
                var json = File.ReadAllText(ConfigFile);
                Web = JsonConvert.DeserializeObject<WebConfig>(json);
            }
        }

        public static string GetAppId() => Web.AppId;
        public static string GetAppSecret() => Web.AppSecret;

        private struct WebConfig {
            public string AppId;
            public string AppSecret;
        }
    }
}