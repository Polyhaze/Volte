using System.IO;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace SIVA.Core.JsonFiles
{
    public class JsonReader
    {
        public static object ReadJson(string fileName)
        {
            var json = JsonConvert.DeserializeObject(fileName);
            return json;
        }

        public static bool WriteJson(string fileName, string fileContents)
        {
            var json = JsonConvert.SerializeObject(fileContents, Formatting.Indented);
            File.WriteAllText(fileName, json);
            return true;
        }
    }
}