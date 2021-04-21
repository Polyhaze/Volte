using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public partial class SettingsModule
    {
        [Command("VerifyRole", "Vr")]
        [Description("Sets or shows the Verified or Unverified roles in this guild.")]
        public Task<ActionResult> VerifyRoleCommandAsync(
            [Description("Valid values are `u` and `v`, for unverified role and verified role respectively.")]
            string type,
            [Remainder, Description("The role to be used.")]
            SocketRole role = null)
        {
            switch (type.ToLower())
            {
                case "v":
                {
                    if (role is null)
                    {
                        role = Context.Guild.GetRole(Context.GuildData.Configuration.Moderation.VerifiedRole);
                        return role is null
                            ? BadRequest("This guild does not have a Verified role set.")
                            : Ok($"The Verified role for this guild is {role.Mention}.");
                    }

                    Context.Modify(data => data.Configuration.Moderation.VerifiedRole = role.Id);
                    return Ok($"Set the Verified role in this guild to {role.Mention}.");
                }
                case "u":
                {
                    if (role is null)
                    {
                        role = Context.Guild.GetRole(Context.GuildData.Configuration.Moderation.UnverifiedRole);
                        return role is null
                            ? BadRequest("This guild does not have an Unverified role set.")
                            : Ok($"The Unverified role for this guild is {role.Mention}.");
                    }

                    Context.Modify(data => data.Configuration.Moderation.UnverifiedRole = role.Id);
                    return Ok($"Set the Unverified role in this guild to {role.Mention}.");
                }
                default:
                    return BadRequest("No Verification role type was specified. Use `u` or `v`.");
            }
        }
    }
}