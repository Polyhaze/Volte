using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Entities;
using Volte.Helpers;

namespace Volte.Commands.Modules
{
    [RequireBotOwner]
    [Group("Set")]
    public sealed class BotOwnerSetModule : VolteModule
    {
        public HttpClient Http { get; set; }

        [Command, DummyCommand,
         Description("The command group for modifying certain parts of the currently logged in bot account.")]
        public async Task<ActionResult> BaseAsync() =>
            Ok(await CommandHelper.CreateCommandEmbedAsync(Context.Command, Context));

        [Command("Game")]
        [Description("Sets the bot's game (presence).")]
        public Task<ActionResult> SetGameAsync([Remainder, Description("The name of the status to set.")]
            string game)
        {
            var activity = Context.Client.Activity;
            return activity.Type is ActivityType.Streaming
                ? Ok($"Set the bot's game to {Format.Bold(game)}.",
                    _ => Context.Client.SetGameAsync(game, activity.Cast<StreamingGame>().Url, activity.Type))
                : Ok($"Set the bot's game to {Format.Bold(game)}.", _ => Context.Client.SetGameAsync(game));
        }

        [Command("Stream")]
        [Description("Sets the bot's stream via Twitch username and Stream name, respectively.")]
        public Task<ActionResult> SetStreamAsync([Description("The Twitch username to link to in the status.")]
            string streamer, [Remainder, Description("The stream title to show.")]
            string game = null)
            => !game.IsNullOrWhitespace()
                ? Ok(
                    $"Set the bot's game to **{game}**, and the Twitch URL to {Format.Bold(Format.Url(streamer, $"https://twitch.tv/{streamer}"))}.",
                    _ => Context.Client.SetGameAsync(game, $"https://twitch.tv/{streamer}", ActivityType.Streaming))
                : Ok(
                    $"Set the bot's stream URL to {Format.Bold(Format.Url(streamer, $"https://twitch.tv/{streamer}"))}.",
                    _ => Context.Client.SetGameAsync(Context.Client.Activity.Name, $"https://twitch.tv/{streamer}",
                        ActivityType.Streaming));

        [Command("Status")]
        [Description("Sets the bot's status.")]
        public Task<ActionResult> SetStatusAsync(
            [Remainder, Description("The status to set. Either `dnd`, `idle`, `invisible`, or `online`.")]
            string status)
            => status.ToLower() switch
            {
                "dnd" => Ok("Set the status to Do Not Disturb.",
                    _ => Context.Client.SetStatusAsync(UserStatus.DoNotDisturb)),
                "idle" => Ok("Set the status to Idle.", _ => Context.Client.SetStatusAsync(UserStatus.Idle)),
                "invisible" => Ok("Set the status to Invisible.",
                    _ => Context.Client.SetStatusAsync(UserStatus.Invisible)),
                "online" => Ok("Set the status to Online.",
                    _ => Context.Client.SetStatusAsync(UserStatus.Online)),
                _ => BadRequest(new StringBuilder()
                    .AppendLine("Your option wasn't known, so I didn't modify the status.")
                    .AppendLine("Available options for this command are `dnd`, `idle`, `invisible`, or `online`.")
                    .ToString())
            };

        [Command("Nickname")]
        [Description("Sets the bot's nickname in the current guild.")]
        public Task<ActionResult> SetNicknameAsync([Remainder, Description("The nickname to use.")]
            string name)
            => Ok($"Set my username to {Format.Bold(name)}.",
                _ => Context.Guild.CurrentUser.ModifyAsync(x => x.Nickname = name));

        [Command("Name")]
        [Description("Sets the bot's username.")]
        public Task<ActionResult> SetNameAsync([Remainder, Description("The username to use.")]
            string name)
            => Ok($"Set my username to {Format.Bold(name)}.",
                _ => Context.Client.CurrentUser.ModifyAsync(u => u.Username = name));

        [Command("Avatar")]
        [Description("Sets the bot's avatar via the given URL, or the attached image.")]
        public async Task<ActionResult> SetAvatarAsync(
            [Remainder,
             Description(
                 "The URL to use as the avatar. Must be a DIRECT image URL. If this URL is not provided the bot will look for a message image attachment.")]
            string url = null)
        {
            if (!Context.Message.Attachments.IsEmpty() && url is null)
                url = Context.Message.Attachments.First().Url;

            if (url.IsNullOrWhitespace() || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return BadRequest("That URL is malformed or empty.");

            var sr = await Http.GetAsync(url);

            return sr.IsImage()
                ? Ok("Done!", async _ =>
                {
                    var img = (await sr.Content.ReadAsByteArrayAsync()).ToStream();
                    await Context.Client.CurrentUser.ModifyAsync(u => u.Avatar = new Image(img));
                })
                : BadRequest(
                    "Provided URL does not lead to an image. Note that I cannot follow redirects; so provide *direct* image URLs please!");
        }
    }
}