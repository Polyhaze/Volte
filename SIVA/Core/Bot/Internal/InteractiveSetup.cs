using System;
using System.Threading;
using Discord;
using Discord.Commands;

namespace SIVA.Core.Bot.Internal
{
    public class InteractiveSetup
    {
        internal static void Setup()
        {
            Console.WriteLine(
                "Hey there! It appears you've not specified a token, so I'm here to help you set that up.");
            Thread.Sleep(1000);
            Console.WriteLine("Please, enter your bot's token after this line.");
            Console.Write("Enter here: ");
            var token = Console.ReadLine();
            Config.bot.Token = token;
            Console.WriteLine("Awesome! Just a few more things to setup and your bot will be up and running!");
            Console.WriteLine(
                "Please enter the prefix you'd like for your commands. This is so you can run things like `{prefix}help`");
            Console.Write("Enter here: ");
            var commandPrefix = Console.ReadLine();
            Config.bot.Prefix = commandPrefix;
            Console.WriteLine(
                "What do you want the bot to set its game as when it logs into Discord? Leave blank to disable.");
            Console.Write("Enter here: ");
            var botGameToSet = Console.ReadLine();
            Config.bot.BotGameToSet = botGameToSet;
            Console.WriteLine(
                "What Twitch streamer to you want to link to when people click the \"Watch\" button on the bot's profile.");
            Console.Write("Enter here: ");
            var twitchStreamer = Console.ReadLine();
            Config.bot.TwitchStreamer = twitchStreamer;
            Console.WriteLine(
                "What symbol do you want set as the currency? So for example it could be 8 :Thonking: if you want thonking as your currency.");
            Console.Write("Enter here: ");
            var currencySymbol = Console.ReadLine();
            Config.bot.CurrencySymbol = currencySymbol;
            Console.WriteLine(
                "The setup has been complete! If you want to further edit your config, head over to Resources/BotConfig.json and open it with Visual Studio Code or something similar.");
            Config.bot.Debug = true;
            Config.bot.DefaultEmbedColour = 0x7000FB;
            Config.bot.ErrorEmbedColour = 0xFF0000;
            Config.bot.LogSeverity = "Verbose";

            Config.SaveConfig();
        }
    }
}