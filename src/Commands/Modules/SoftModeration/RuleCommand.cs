using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace BrackeysBot.Commands
{
    public partial class SoftModerationModule : BrackeysBotModule
    {
        [Command("rule")]
        [Remarks("rule <id>")]
        [Summary("Displays the rule with the given ID.")]
        public async Task DisplayRuleAsync(
            [Summary("The ID of the rule to display.")] int id)
        {
            string[] rules = GetRuleArray();
            int index = id - 1;

            EmbedBuilder builder = GetDefaultBuilder()
                .WithTitle($"Rule #{id}")
                .WithFooter("Go to #info for a list of all the rules!");

            if (rules != null && index >= 0 && index < rules.Length)
            {
                builder.WithDescription(rules[index]);
            }
            else
            {
                builder.WithDescription($"A rule with the ID {id} does not exist.")
                    .WithColor(Color.Red);
            }

            await builder.Build().SendToChannel(Context.Channel);
        }

        [Command("rules")]
        [Summary("Displays a list of all the rules.")]
        [RequireModerator]
        public async Task DisplayRulesAsync()
        {
            string[] rules = GetRuleArray();

            EmbedBuilder builder = GetDefaultBuilder();

            if (rules != null && rules.Length > 0)
            {
                builder.WithDescription(string.Join('\n', rules.Select((r, i) => $"{i + 1} - {r}").ToArray()))
                    .WithTitle("Rules");
            }
            else
            {
                builder.WithDescription("No rules exist yet.")
                    .WithColor(Color.Red);
            }

            await builder.Build().SendToChannel(Context.Channel);
        }

        [Command("setrule"), Alias("addrule"), Priority(1)]
        [Summary("Adds or updates a rule based on the ID.")]
        [Remarks("setrule <id> <content>")]
        [RequireModerator]
        public async Task SetRuleAsync(
            [Summary("The ID of the rule to edit.")] int id,
            [Summary("The new content of the rule."), Remainder] string content)
        {
            string[] rules = GetRuleArray() ?? new string[0];

            if (id <= 0)
                throw new ArgumentException("The rule ID cannot be zero or negative.");
            if (id > rules.Length)
                Array.Resize(ref rules, id);

            rules[id - 1] = content;
            SetRuleArray(rules);

            await GetDefaultBuilder()
                .WithDescription($"Rule {id} was updated!")
                .Build().SendToChannel(Context.Channel);
        }

        [Command("addrule")]
        [Summary("Adds a rule.")]
        [Remarks("addrule <content>")]
        [RequireModerator]
        public async Task AddRuleAsync(
            [Summary("The content of the rule to add."), Remainder] string content)
        {
            int id = (GetRuleArray()?.Length ?? 0) + 1;
            await SetRuleAsync(id, content);
        }

        private string[] GetRuleArray()
            => Data.Configuration.Rules;
        private void SetRuleArray(string[] rules)
            => Data.Configuration.Rules = rules;
    }
}
