using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Helpers;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("SetAvatar")]
        [Summary("Sets the bot's avatar.")]
        [Remarks("Usage: $setavatar {url}")]
        [RequireBotOwner]
        public async Task SetAvatar(string url) {
            if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute)) {
                await Reply(Context.Channel, CreateEmbed(Context, "That URL is malformed or empty."));
                return;
            }

            using (var sr = await new HttpClient().GetAsync(new Uri(url), HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false)) {
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