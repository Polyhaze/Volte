using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("ModRole")]
        [Description("Sets the role able to use Moderation commands for the current guild.")]
        [Remarks("Usage: |prefix|modrole {roleName}")]
        [RequireGuildAdmin]
        public async Task ModRole([Remainder] string roleName)
        {
            var config = Db.GetConfig(Context.Guild);
            var embed = Context.CreateEmbed(string.Empty).ToEmbedBuilder();

            if (Context.Guild.Roles.Any(r => r.Name.EqualsIgnoreCase(roleName)))
            {
                var role = Context.Guild.Roles.First(r => r.Name.EqualsIgnoreCase(roleName));
                config.ModRole = role.Id;
                Db.UpdateConfig(config);
                embed.WithDescription($"Set **{role.Name}** as the Moderator role for this server.");
            }
            else
            {
                embed.WithDescription($"{roleName} doesn't exist in this server.");
            }

            await embed.SendTo(Context.Channel);
        }
    }
}