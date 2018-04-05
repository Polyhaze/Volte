using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SIVA.Core.JsonFiles
{
    public class TruthOrDare
    {
        public TruthOrDare()
        {
            Truths = new List<string>();
            Dares = new List<string>();
        }

        public List<string> Truths { get; set; }
        public List<string> Dares { get; set; }
    }

    public class TruthOrDareJson
    {
        static TruthOrDareJson()
        {
            try
            {
                var jsonText = File.ReadAllText(filePath);
                TruthOrDare = JsonConvert.DeserializeObject<List<TruthOrDare>>(jsonText);
            }
            catch (Exception)
            {
                File.Create("Resources/TruthOrDare.json");
            }
        }

        private static readonly List<TruthOrDare> TruthOrDare = new List<TruthOrDare>();
        private static string filePath = "Resources/TruthOrDare.json";

        public static TruthOrDare LoadJson()
        {
            return TruthOrDare.First();
        }

        /*public static void AddTruthToJson(string truth)
        {
            LoadJson().Truths.Add(truth);
            var json = JsonConvert.SerializeObject(TruthOrDare, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static void AddDareToJson(string dare)
        {
            LoadJson().Dares.Add(dare);
            var json = JsonConvert.SerializeObject(TruthOrDare, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }*/
    }
}
