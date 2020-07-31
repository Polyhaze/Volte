using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using BrackeysBot.Services;

namespace BrackeysBot.Commands
{
    public partial class ConfigurationModule : BrackeysBotModule
    {
        public ConfigurationService Config { get; set; }

        [Command("config"), Alias("configuration", "c")]
        [Remarks("config [name] [value]")]
        [Summary("Shows the entire configuration, or just a single value, or changes a value.")]
        [RequireModerator]
        public async Task DisplayConfigAllAsync()
        {
            EmbedBuilder builder = GetDefaultBuilder()
                .WithTitle("Configuration")
                .WithFields(Config.GetConfigurationValues()
                    .Select(v => new EmbedFieldBuilder()
                        .WithName(v.Name)
                        .WithValue(LimitFieldLength(v.ToString()))
                        .WithIsInline(true)));

            await builder.Build().SendToChannel(Context.Channel);
        }

        [Command("config"), Alias("configuration", "c", "viewconfig")]
        [Remarks("config <name>")]
        [Summary("Displays information about a value in the configuration.")]
        [RequireModerator]
        [HideFromHelp]
        public async Task DisplayConfigAsync(
            [Summary("The name of the configuration entry.")] string name)
        {
            EmbedBuilder builder = GetDefaultBuilder();

            if (Config.TryGetValue(name, out ConfigurationValue configValue))
            {
                builder.WithTitle(configValue.Name)
                    .WithDescription(configValue.Description)
                    .AddField("Value", LimitFieldLength(configValue.ToString()));
            }
            else
            {
                builder.WithDescription($"A configuration entry with the name of {name} could not be found.")
                    .WithColor(Color.DarkRed);
            }

            await builder.Build().SendToChannel(Context.Channel);
        }

        [Command("config"), Alias("configuration", "c", "setconfig")]
        [Remarks("config <name> <value>")]
        [Summary("Sets the value of a configuration.")]
        [RequireModerator]
        [HideFromHelp]
        public async Task SetConfigAsync(
            [Summary("The name of the configuration entry.")] string name, 
            [Summary("The value to set the entry to."), Remainder] string value)
        {
            EmbedBuilder builder = GetDefaultBuilder();

            if (Config.TryGetValue(name, out ConfigurationValue configValue))
            {
                if (configValue.SetValue(value))
                {
                    builder.WithDescription($"Successfully set **{configValue.Name}** to `{value}`.")
                        .WithColor(Color.Green);
                }
                else
                {
                    builder.WithDescription($"Something went wrong while changing the value of {configValue.Name}.")
                        .WithColor(Color.Red);
                }
            }
            else
            {
                builder.WithDescription($"A configuration entry with the name of {name} could not be found.")
                    .WithColor(Color.DarkRed);
            }

            await builder.Build().SendToChannel(Context.Channel);
        }

        [Command("saveconfig"), Alias("saveconfiguration", "sc")]
        [Summary("Saves the configuration of the bot to the config file.")]
        [RequireModerator]
        public async Task SaveConfigAsync()
        {
            Config.Save();

            await GetDefaultBuilder()
                .WithDescription("Configuration was saved!")
                .Build()
                .SendToChannel(Context.Channel);
        }

        private static string LimitFieldLength(string content)
        {
            const int maxLength = EmbedFieldBuilder.MaxFieldValueLength;
            const string truncator = "[...]";
            if (content.Length > maxLength)
            {
                content = content[0..(maxLength - truncator.Length)] + truncator;
            }
            return content;
        }
    }
}
