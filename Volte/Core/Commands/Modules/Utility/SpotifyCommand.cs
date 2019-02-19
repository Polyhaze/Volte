using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Humanizer;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Spotify")]
        [Description("Shows what you're listening to on Spotify, if you're listening to something.")]
        [Remarks("Usage: |prefix|spotify")]
        public async Task Spotify()
        {
            if (Context.User.Activity is SpotifyGame spotify)
            {
                var e = Context.CreateEmbedBuilder(string.Empty)
                    .WithDescription($"**Track:** [{spotify.TrackTitle}]({spotify.TrackUrl})\n" +
                                     $"**Album:** {spotify.AlbumTitle}\n" +
                                     $"**Duration:** {spotify.Duration}\n" +
                                     $"**Artists:** {string.Join(',', spotify.Artists)}")
                    .WithThumbnailUrl(spotify.AlbumArtUrl)
                    .SendTo(Context.Channel);
                return;

            }
            await Context.CreateEmbed("You're not listening to Spotify right now.").SendTo(Context.Channel);
        }
    }
}
