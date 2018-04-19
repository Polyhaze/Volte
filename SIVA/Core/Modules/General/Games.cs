using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using SIVA.Core.JsonFiles;
using System;
using SIVA.Core.Bot;

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

            EmbedBuilder embed = Helpers.CreateEmbed(Context, truth);

            await Helpers.SendMessage(Context, embed);

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

            EmbedBuilder embed = Helpers.CreateEmbed(Context, dare);

            await Helpers.SendMessage(Context, embed);

        }
    }
}