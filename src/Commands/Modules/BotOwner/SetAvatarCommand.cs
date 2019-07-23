using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using RestSharp;
using Volte.Commands.Checks;
using Volte.Core.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule : VolteModule
    {
        public RestClient Http { get; set; }

        [Command("SetAvatar")]
        [Description("Sets the bot's avatar.")]
        [Remarks("Usage: |prefix|setavatar {url}")]
        [RequireBotOwner]
        public async Task<VolteCommandResult> SetAvatarAsync(string url)
        {
            if (url.IsNullOrWhitespace() || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return BadRequest("That URL is malformed or empty.");
            }

            using (var sr = await Http.GetAsync<HttpResponseMessage>(new RestRequest(url)))
            {
                if (!sr.IsImage())
                {
                    return BadRequest(
                        "Provided URL does not lead to an image. Note that I cannot follow redirects; so provide *direct* image URLs please!");
                }

                using (var img = (await sr.Content.ReadAsByteArrayAsync()).ToStream())
                {
                    await Context.Client.CurrentUser.ModifyAsync(u => u.Avatar = new Image(img));
                    return Ok("Done!");
                }
            }
        }
    }
}