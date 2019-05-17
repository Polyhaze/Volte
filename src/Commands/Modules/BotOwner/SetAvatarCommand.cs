using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;
using Gommon;

namespace Volte.Commands.Modules.BotOwner
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("SetAvatar")]
        [Description("Sets the bot's avatar.")]
        [Remarks("Usage: |prefix|setavatar {url}")]
        [RequireBotOwner]
        public async Task SetAvatarAsync(string url)
        {
            if (url.IsNullOrWhitespace() || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                await Context.CreateEmbed("That URL is malformed or empty.").SendToAsync(Context.Channel);
                return;
            }

            using (var sr = await new HttpClient().GetAsync(new Uri(url), HttpCompletionOption.ResponseHeadersRead))
            {
                if (!sr.IsImage())
                {
                    await Context.CreateEmbed("Provided URL does not lead to an image.").SendToAsync(Context.Channel);
                    return;
                }

                using (var img = (await sr.Content.ReadAsByteArrayAsync()).ToStream())
                {
                    await Context.Client.CurrentUser.ModifyAsync(u => u.Avatar = new Image(img));
                    await Context.CreateEmbed("Done!").SendToAsync(Context.Channel);
                }
            }
        }
    }
}