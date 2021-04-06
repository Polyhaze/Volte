using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;

namespace Volte.Commands.Modules
{
    public partial class AdminUtilityModule
    {
        [Command("Setup")]
        [Description("Interactively setup your guild's moderator and admin roles.")]
        public Task<ActionResult> SetupVolteAsync()
        {
            return Ok(async () =>
            {
                adminRole:
                await Context.CreateEmbed("What role would you like to have Admin permission with me?")
                    .SendToAsync(Context.Channel);
                var (role, didTimeout) = await GetAsync<SocketRole>();
                if (didTimeout) return;
                if (role is null) goto adminRole;

                Context.GuildData.Configuration.Moderation.AdminRole = role.Id;
                Db.Save(Context.GuildData);
                
                modRole:
                await Context.CreateEmbed("What role would you like to have Moderator permission with me?")
                    .SendToAsync(Context.Channel);
                (role, didTimeout) = await GetAsync<SocketRole>();
                if (didTimeout) return;
                if (role is null) goto modRole;

                Context.GuildData.Configuration.Moderation.ModRole = role.Id;
                Db.Save(Context.GuildData);
                
                await Context.CreateEmbed("Done. People with those roles can now use their respective commands.")
                    .SendToAsync(Context.Channel);

            }, false);
        }
    }
}