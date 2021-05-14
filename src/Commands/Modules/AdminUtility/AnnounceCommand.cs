using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("Announce")]
        [Description("Allows you to construct a custom embed from Unix-style command arguments.")]
        [ShowUnixArgumentsInHelp(VolteUnixCommand.Announce)]
        public async Task<ActionResult> AnnounceAsync(
            [Remainder,
             Description(
                 "Unix-style command line arguments. Example: `-description=\"Some cool thing!\"` will set the embed's description.")]
            Dictionary<string, string> options)
        {
            static string GetRoleMention(TypeParserResult<SocketRole> res) =>
                res.IsSuccessful ? res.Value.Mention : null;

            static Color GetColor(TypeParserResult<Color> res) =>
                res.IsSuccessful ? res.Value : new Color(Config.SuccessColor);

            static bool TryGetUser(TypeParserResult<RestGuildUser> res, out RestGuildUser user)
            {
                user = res.IsSuccessful ? res.Value : null;
                return user != null;
            }

            var embed = new EmbedBuilder();

            if (options.TryGetValue("footer", out var result) || options.TryGetValue("foot", out result))
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
            {
                //must be a URL
                if (Uri.IsWellFormedUriString(WebUtility.UrlEncode(result), UriKind.RelativeOrAbsolute)
                    //must be a website/paste service that has support for raw paste viewing via a URL; feel free to PR more or to message me on discord to add some
                    && result.ContainsAnyIgnoreCase(AllowedPasteSites) 
                    //must be a url that leads to plaintext (aka raw on most websites) so it's not a bunch of HTML as the result.
                    && result.ContainsIgnoreCase("raw"))
                {
                    try
                    {
                        var m = await Http.GetAsync(WebUtility.UrlEncode(result));
                        result = await m.Content.ReadAsStringAsync();
                    }
                    catch { /* ignored */ }
                }
                
                embed.WithDescription(result);
            }

            if (options.TryGetValue("title", out result))
                embed.WithTitle(result);

            if (options.TryGetValue("color", out result) || options.TryGetValue("colour", out result))
                embed.WithColor(GetColor(await CommandService.GetTypeParser<Color>()
                    .ParseAsync(null, result, Context)));

            if (options.TryGetValue("author", out result))
            {
                if (result.EqualsAnyIgnoreCase("self", "me"))
                    embed.WithAuthor(Context.User);
                else if (result.EqualsAnyIgnoreCase("bot", "you", "volte"))
                    embed.WithAuthor(Context.Guild.CurrentUser);
                else if (TryGetUser(await CommandService.GetTypeParser<RestGuildUser>()
                    .ParseAsync(null, result, Context), out var user))
                    embed.WithAuthor(user);
            }

            var mention = options.TryGetValue("mention", out result) || options.TryGetValue("ping", out result)
                ? result switch
                {
                    "none" => null,
                    "everyone" => "@everyone",
                    "here" => "@here",
                    _ => GetRoleMention(await CommandService.GetTypeParser<SocketRole>()
                        .ParseAsync(null, result, Context))
                }
                : null;

            return Ok(async () =>
            {
                async Task<RestUserMessage> SendResultAsync()
                {
                    try
                    {
                        return await Context.Channel.SendMessageAsync(mention, embed: embed.Build());
                    }
                    catch (HttpException)
                    {
                        return await Context.Channel.SendMessageAsync(
                            embed: embed.WithTitle("You need to modify the embed in some way.").Build());
                    }
                }

                var m = await SendResultAsync();
                
                if (!(options.TryGetValue("keepmessage", out _) || options.TryGetValue("keepmsg", out _))
                    && Context.Guild.CurrentUser.GetPermissions(Context.Channel).ManageMessages)
                    await Context.Message.TryDeleteAsync();
                if ((options.TryGetValue("publish", out _) || options.TryGetValue("crosspost", out _))
                    && Context.Channel is INewsChannel)
                    await m.CrosspostAsync();
            });
        }
    }
}