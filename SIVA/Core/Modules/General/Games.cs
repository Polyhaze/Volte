using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using SIVA.Core.JsonFiles;
using System;

namespace SIVA.Core.Modules.General
{
    public class Games : ModuleBase<SocketCommandContext>
    {
        [Command("Truth")]
        public async Task LoadTruthFromJson()
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            if (!config.IsTodEnabled) return;
            var json = TruthOrDareJson.LoadJson();
            var r = new Random().Next(0, json.Truths.Count);
            var truthsList = json.Truths.ToArray();
            var truth = truthsList[r];

            var embed = new EmbedBuilder()
                .WithDescription(truth)
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await ReplyAsync("", false, embed);
            
        }

        [Command("Dare")]
        public async Task LoadDareFromJson()
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            if (!config.IsTodEnabled) return;
            var json = TruthOrDareJson.LoadJson();
            var r = new Random().Next(0, json.Dares.Count);
            var daresList = json.Dares.ToArray();
            var dare = daresList[r];

            var embed = new EmbedBuilder()
                .WithDescription(dare)
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await ReplyAsync("", false, embed);

        }
    }
}