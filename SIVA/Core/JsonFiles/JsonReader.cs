using System;
using System.IO;
using Newtonsoft.Json;

namespace SIVA.Core.JsonFiles
{
    public class JsonReader
    {
        public static object ReadJson(string fileName)
        {
            object json = JsonConvert.DeserializeObject(fileName);
            return json;
        }

        public static bool WriteJson(string fileName, string fileContents)
        {
            string json = JsonConvert.SerializeObject(fileContents, Formatting.Indented);
            File.WriteAllText(fileName, json);
            return true;
        }
    }
}