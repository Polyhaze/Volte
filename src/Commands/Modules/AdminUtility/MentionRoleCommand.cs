using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("MentionRole", "Menro")]
        [Description(
            "Mentions a role. If it isn't mentionable, it allows it to be, mentions it, and then undoes the first action.")]
        public Task<ActionResult> MentionRoleAsync([Remainder, Description("The role to mention.")]
            SocketRole role)
        {
            return role.IsMentionable
                ? Ok(role.Mention, shouldEmbed: false)
                : Ok(async () =>
                {
                    await role.ModifyAsync(x => x.Mentionable = true);
                    await Context.Channel.SendMessageAsync(role.Mention);
                    await role.ModifyAsync(x => x.Mentionable = false);
                });
        }
    }
}