using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("Announce")]
        [Description("Allows you to construct a custom embed from Unix-style command arguments.")]
        public async Task<ActionResult> AnnounceAsync(
            [Remainder,
             Description(
                 "Unix-style command line arguments. Example: `-description=\"Some cool thing!\"` will set the embed's description.")]
            Dictionary<string, string> options)
        {
            var embed = new EmbedBuilder();
            var shouldPublish = options.TryGetValue("publish", out _) || options.TryGetValue("crosspost", out _);

            string GetRoleMention(TypeParserResult<SocketRole> res) => res.IsSuccessful ? res.Value.Mention : null;

            Color GetColor(TypeParserResult<Color> res) =>
                res.IsSuccessful ? res.Value : new Color(Config.SuccessColor);

            RestGuildUser GetUser(TypeParserResult<RestGuildUser> res) => res.IsSuccessful ? res.Value : null;

            string toMention = null;

            if (options.TryGetValue("mention", out var result) || options.TryGetValue("ping", out result))
            {
                toMention = result switch
                {
                    "none" => null,
                    "everyone" => "@everyone",
                    "here" => "@here",
                    _ => GetRoleMention(await CommandService.GetTypeParser<SocketRole>()
                        .ParseAsync(null, result, Context))
                };
            }

            if (options.TryGetValue("footer", out result) || options.TryGetValue("foot", out result))
                embed.WithFooter(result);

            if (options.TryGetValue("thumbnail", out result))
            {
                if (!Uri.IsWellFormedUriString(result, UriKind.Absolute))
                    return BadRequest("Thumbnail URL must be a valid image URL.");

                embed.WithThumbnailUrl(result);
            }

            if (options.TryGetValue("image", out result))
            {
                if (!Uri.IsWellFormedUriString(result, UriKind.Absolute))
                    return BadRequest("Image URL must be a valid image URL.");

                embed.WithImageUrl(result);
            }

            if (options.TryGetValue("description", out result) || options.TryGetValue("desc", out result))
                embed.WithDescription(result);

            if (options.TryGetValue("title", out result))
                embed.WithTitle(result);

            if (options.TryGetValue("color", out result) || options.TryGetValue("colour", out result))
                embed.WithColor(GetColor(await CommandService.GetTypeParser<Color>()
                    .ParseAsync(null, result, Context)));

            if (options.TryGetValue("author", out result))
            {
                if (result.EqualsIgnoreCase("self") || result.EqualsIgnoreCase("me"))
                    embed.WithAuthor(Context.User);
                else if (result.EqualsIgnoreCase("bot")
                         || result.EqualsIgnoreCase("you")
                         || result.EqualsIgnoreCase("volte"))
                    embed.WithAuthor(Context.Guild.CurrentUser);
                else
                {
                    var user = GetUser(await CommandService.GetTypeParser<RestGuildUser>()
                        .ParseAsync(null, result, Context));
                    if (user != null)
                        embed.WithAuthor(user);
                }
            }

            return Ok(async () =>
            {
                var m = await Context.Channel.SendMessageAsync(toMention, embed: embed.Build());
                await Context.Message.TryDeleteAsync();
                if (shouldPublish) await m.CrosspostAsync();
            });
        }
    }
}