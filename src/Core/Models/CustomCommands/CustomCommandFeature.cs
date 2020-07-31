using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using Discord.Commands;

namespace BrackeysBot.Core.Models
{
    public abstract class CustomCommandFeature
    {
        [JsonIgnore] public string Name => GetName(GetType());
        [JsonIgnore] public string Summary => GetSummary(GetType());

        public abstract void FillArguments(string arguments);
        public abstract Task Execute(ICommandContext context);

        public static string GetName(Type featureType)
            => featureType.GetCustomAttribute<NameAttribute>()?.Text ?? featureType.Name;
        public static string GetSummary(Type featureType)
            => featureType.GetCustomAttribute<SummaryAttribute>()?.Text ?? string.Empty;
    }
}
