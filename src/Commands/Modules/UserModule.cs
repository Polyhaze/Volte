using Discord.Commands;

using BrackeysBot.Services;

namespace BrackeysBot.Commands
{
    [ModuleColor(0xeda532)]
    [Summary("Provides services for users to customize themselves!")]
    public partial class UserModule : BrackeysBotModule
    {
        public SelfUserService UserService { get; set; }
    }
}