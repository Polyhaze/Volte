using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Helpers;
using Volte.Core.Extensions;

namespace Volte.Core.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("SetAvatar")]
        [Summary("Sets the bot's avatar.")]
        [Remarks("Usage: $setavatar {url}")]
        public async Task SetAvatar(string url) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute)) {
                await Reply(Context.Channel, CreateEmbed(Context, "That URL is malformed or empty."));
                return;
            }
             
            var uri = new Uri(url);

            using (var sr = await new HttpClient().GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false)) {
                if (!sr.IsImage()) {
                    await Reply(Context.Channel, CreateEmbed(Context, "Provided URL does not lead to an image."));
                    return;
                }

                using (var img = (await sr.Content.ReadAsByteArrayAsync()).ToStream()) {
                    await Context.Client.CurrentUser.ModifyAsync(u => u.Avatar = new Image(img));
                    await Reply(Context.Channel, CreateEmbed(Context, "Done!"));
                }
            }
        }
    }
}