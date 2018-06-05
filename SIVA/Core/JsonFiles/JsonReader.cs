using System;
using System.IO;
using Newtonsoft.Json;

namespace SIVA.Core.JsonFiles
{
    public class JsonReader
    {
        public static object ReadJson(string fileName)
        {
            var json = File.ReadAllText(fileName);
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            return data;
        }

        public static bool WriteJson(string fileName, string fileContents)
        {
            try
            {
                var json = JsonConvert.SerializeObject(fileContents, Formatting.Indented);
                File.WriteAllText(fileName, json);
                return true;
            }
            catch
            {
                throw new Exception("The operation failed to complete.");
            }
        }
    }
}