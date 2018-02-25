using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    public class Owner : ModuleBase<SocketCommandContext>
    {

        [Command("Shutdown")]
        [RequireOwner]
        public async Task Shutdown()
        {
            
            var _client = new DiscordSocketClient();

            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedAlert("LoggingOutMsg", Context.User.Mention));
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            await Context.Channel.SendMessageAsync("", false, embed);
            await _client.LogoutAsync();
            await _client.StopAsync();

        }

        [Command("Game")]
        [RequireOwner]
        public async Task SetBotGame([Remainder] string game)
        {
            var client = new DiscordSocketClient();

            var embed = new EmbedBuilder();
            embed.WithDescription($"Set the bot's game to {game}");
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            await client.SetGameAsync(game);
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("Status")]
        [RequireOwner]
        public async Task SetBotStatus(string status)
        {
            var embed = new EmbedBuilder();
            embed.WithDescription($"Set the status to {status}");
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.defaultEmbedColour);

            var client = new DiscordSocketClient();

            switch (status)
            {
                case "dnd":
                    await client.SetStatusAsync(UserStatus.DoNotDisturb);
                    break;
                case "idle":
                    await client.SetStatusAsync(UserStatus.Idle);
                    break;
                case "online":
                    await client.SetStatusAsync(UserStatus.Online);
                    break;
                case "offline":
                    await client.SetStatusAsync(UserStatus.Invisible);
                    break;
            }

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("LeaveServer")]
        [RequireOwner]
        public async Task LeaveServer()
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetAlert("BotLeftServer"));
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            await Context.Channel.SendMessageAsync("", false, embed);
            await Context.Guild.LeaveAsync();
        }

        /*[Command("Image")]
        [RequireOwner]
        public async Task SetBotImage(string url)
        {
            
        }*/
    }
}
