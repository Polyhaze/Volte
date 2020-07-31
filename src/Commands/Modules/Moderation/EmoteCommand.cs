using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace BrackeysBot.Commands
{
    public partial class ModerationModule : BrackeysBotModule
    {
        [Command("limitemote")]
        [Summary("List the emote limits of the current channel")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        [HideFromHelp]
        public async Task EmoteLimitAsync() 
            => await EmoteLimitAsync(Context.Channel as ITextChannel);

        [Command("limitemote")]
        [Summary("List the emote limits of the given channel, or current channel if omitted")]
        [Remarks("limitemote [channel]")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task EmoteLimitAsync(
            [Summary("The channel to display the limits for")] ITextChannel channel)
        {
            if (channel == null)
                throw new ArgumentException("Given channel does not exist");
            
            Dictionary<ulong, List<string>> limits = Data.Configuration.EmoteRestrictions;
            EmbedBuilder builder = new EmbedBuilder();

            if (limits == null || !limits.ContainsKey(channel.Id) || limits.GetValueOrDefault(channel.Id).Count() == 0)
            {
                builder.WithColor(Color.Red)
                    .WithDescription($"No restrictions in {channel.Mention}!");
            } 
            else 
            {
                List<string> limitedTo = limits.GetValueOrDefault(channel.Id);

                builder.WithColor(Color.Green)
                    .WithDescription($"Channel {channel.Mention} limits to {string.Join(", ", limitedTo)}");
            }
            
            await builder.Build()
                .SendToChannel(Context.Channel);
        }
    
        [Command("addlimitemote")]
        [Summary("Add an emote to the allowed list in the given channel")]
        [Remarks("addlimitemote <channel> <emote>")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task AddEmoteLimitAsync(
            [Summary("The channel to display the limits for")] ITextChannel channel,
            [Summary("The emote to allow"), OverrideTypeReader(typeof(EmoteTypeReader))] IEmote emote)
        {
            if (channel == null)
                throw new ArgumentException("Given Channel does not exist");

            if (emote == null) 
                throw new ArgumentException("Given Emote does not exist");
            
            Dictionary<ulong, List<string>> limits = Data.Configuration.EmoteRestrictions;

            if (limits == null) 
                limits = new Dictionary<ulong, List<string>>();

            List<string> newLimits = limits.GetValueOrDefault(channel.Id, new List<string>());

            if (newLimits.Contains(emote.StringVal()))
                throw new ArgumentException($"Emote {emote.Emotify()} is aleady permitted");

            newLimits.Add(emote.StringVal());

            Console.WriteLine(newLimits);

            if (limits.ContainsKey(channel.Id)) 
                limits.Remove(channel.Id);

            limits.Add(channel.Id, newLimits);

            await new EmbedBuilder().WithColor(Color.Green)
                .WithDescription($"Channel {channel.Mention} now limits to {string.Join(", ", newLimits.ConvertAll(ConvertToVisual))}")
                .Build()
                .SendToChannel(Context.Channel);

            Data.Configuration.EmoteRestrictions = limits;
            Data.SaveConfiguration();
        }

        [Command("dellimitemote")]
        [Summary("Delete an emote from the allowed list in the given channel")]
        [Remarks("dellimitemote <channel> <emote>")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task DeleteEmoteLimitAsync(
            [Summary("The channel to display the limits for")] ITextChannel channel,
            [Summary("The emote to allow"), OverrideTypeReader(typeof(EmoteTypeReader))] IEmote emote)
        {
            if (channel == null)
                throw new ArgumentException("Given Channel does not exist");

            if (emote == null) 
                throw new ArgumentException("Given Emote does not exist");
            
            Dictionary<ulong, List<string>> limits = Data.Configuration.EmoteRestrictions;

            if (limits == null) 
                throw new ArgumentException($"Channel {channel.Mention} has no emote restrictions");

            List<string> newLimits = limits.GetValueOrDefault(channel.Id, new List<string>());

            if (!newLimits.Remove(emote.StringVal()))
                throw new ArgumentException($"Channel {channel.Mention} does not allow emote {emote.Emotify()}");

            if (limits.ContainsKey(channel.Id)) 
                limits.Remove(channel.Id);

            limits.Add(channel.Id, newLimits);

            await new EmbedBuilder().WithColor(Color.Green)
                .WithDescription($"Channel {channel.Mention} now limits to {string.Join(", ", newLimits.ConvertAll(ConvertToVisual))}")
                .Build()
                .SendToChannel(Context.Channel);

            Data.Configuration.EmoteRestrictions = limits;
            Data.SaveConfiguration();
        }

        private string ConvertToVisual(string emoteStr) => emoteStr.Contains(':') ? $"<{emoteStr}>" : emoteStr;
    }
}
