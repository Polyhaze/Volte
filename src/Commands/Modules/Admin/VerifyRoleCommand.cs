using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Attributes;

namespace Volte.Commands.Modules
{
    public partial class AdminModule
    {
        [Command("VerifyRole", "Vr")]
        [Description("Sets or shows the Verified or Unverified roles in this guild. v/u is the role type, v is for Verified and u is for Unverified.")]
        [Remarks("verifyrole {v/u} [Role]")]
        [RequireGuildAdmin]
        public Task<ActionResult> VerifyRoleCommandAsync(string type, [Remainder] SocketRole role = null)
        {
            switch (type.ToLower())
            {
                case "v":
                {
                    if (role is null)
                    {
                        role = Context.Guild.GetRole(Context.GuildData.Configuration.Moderation.VerifiedRole);
                        if (role is null)
                            return BadRequest("This guild does not have a Verified role set.");
                        return Ok($"The Verified role for this guild is {role.Mention}.");
                    }

                    Context.GuildData.Configuration.Moderation.VerifiedRole = role.Id;
                    Db.UpdateData(Context.GuildData);
                    return Ok($"Set the Verified role in this guild to {role.Mention}.");
                }
                case "u":
                {
                    if (role is null)
                    {
                        role = Context.Guild.GetRole(Context.GuildData.Configuration.Moderation.UnverifiedRole);
                        if (role is null)
                            return BadRequest("This guild does not have an Unverified role set.");
                        return Ok($"The Unverified role for this guild is {role.Mention}.");
                    }

                    Context.GuildData.Configuration.Moderation.UnverifiedRole = role.Id;
                    Db.UpdateData(Context.GuildData);
                    return Ok($"Set the Unverified role in this guild to {role.Mention}.");
                }
                default:
                    return BadRequest("No Verification role type was specified. Use `u` or `v`.");
            }
        }
    }
}