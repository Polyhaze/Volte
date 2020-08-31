using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;
using Volte.Core.Entities.Attributes;

namespace Volte.Commands.Modules
{
    [RequireGuildAdmin]
    public sealed class AdminUtilityModule : VolteModule 
    {
        [Command("GuildName", "Gn")]
        [Description("Sets the name of the current guild.")]
        [Remarks("guildname {String}")]
        [RequireBotGuildPermission(Permissions.ManageGuild)]
        public async Task<ActionResult> GuildNameAsync([Remainder, RequiredArgument] string name)
        {
            await Context.Guild.ModifyAsync(g => g.Name = name);
            return Ok($"Set this guild's name to **{name}**!");
        }

        [Command("RoleColor", "RoleClr", "Rcl")]
        [Description("Changes the color of a specified role. Accepts a Hex or RGB value.")]
        [Remarks("rolecolor {Role} {Color}")]
        [RequireBotGuildPermission(Permissions.ManageRoles)]
        public async Task<ActionResult> RoleColorAsync([RequiredArgument] DiscordRole role, [Remainder, RequiredArgument] DiscordColor color)
        {
            await role.ModifyAsync(x => x.Color = color);
            return Ok($"Successfully changed the color of the role **{role.Name}**.");
        }

        [Command("MentionRole", "Menro", "Mr")]
        [Description(
            "Mentions a role. If it isn't mentionable, it allows it to be, mentions it, and then undoes the first action.")]
        [Remarks("mentionrole {Role}")]
        [RequireBotGuildPermission(Permissions.ManageRoles)]
        public Task<ActionResult> MentionRoleAsync([Remainder, RequiredArgument] DiscordRole role)
        {
            if (role.IsMentionable)
            {
                return Ok(role.Mention, shouldEmbed: false);
            }

            return Ok(async () =>
            {
                await role.ModifyAsync(x => x.Mentionable = true);
                await Context.ReplyAsync(role.Mention);
                await role.ModifyAsync(x => x.Mentionable = false);
            });
        }

        [Command("ChannelName", "Cn")]
        [Description("Sets the name of the current channel. Replaces all spaces with a -.")]
        [Remarks("channelname {String}")]
        [RequireBotChannelPermission(Permissions.ManageChannels)]
        public async Task<ActionResult> ChannelNameAsync([Remainder, RequiredArgument] string name)
        {
            await Context.Channel.ModifyAsync(c => c.Name = name.Replace(" ", "-"));
            return Ok($"Set this channel's name to **{name}**.");
        }
    }
}