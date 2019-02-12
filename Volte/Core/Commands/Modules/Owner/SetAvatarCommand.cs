using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("SetAvatar")]
        [Summary("Sets the bot's avatar.")]
        [Remarks("Usage: $setavatar {url}")]
        [RequireBotOwner]
        public async Task SetAvatar(string url) {
            if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute)) {
                await Context.CreateEmbed("That URL is malformed or empty.").SendTo(Context.Channel);
                return;
            }

            using (var sr = await new HttpClient().GetAsync(new Uri(url), HttpCompletionOption.ResponseHeadersRead)) {
                if (!sr.IsImage()) {
                    await Context.CreateEmbed("Provided URL does not lead to an image.").SendTo(Context.Channel);
                    return;
                }

                using (var img = (await sr.Content.ReadAsByteArrayAsync()).ToStream()) {
                    await Context.Client.CurrentUser.ModifyAsync(u => u.Avatar = new Image(img));
                    await Context.CreateEmbed("Done!").SendTo(Context.Channel);
                }
            }
        }
    }
}