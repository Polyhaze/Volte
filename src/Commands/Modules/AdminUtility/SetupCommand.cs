using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Helpers;

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
                var (role, didTimeout) = await Context.GetNextAsync<SocketRole>();
                if (didTimeout) return;
                if (role is null) goto adminRole;

                // ReSharper disable twice AccessToModifiedClosure
                Context.Modify(data => data.Configuration.Moderation.AdminRole = role.Id);

                modRole:
                await Context.CreateEmbed("What role would you like to have Moderator permission with me?")
                    .SendToAsync(Context.Channel);
                (role, didTimeout) = await Context.GetNextAsync<SocketRole>();
                if (didTimeout) return;
                if (role is null) goto modRole;

                Context.Modify(data => data.Configuration.Moderation.ModRole = role.Id);
                
                await Context.CreateEmbed("Done. People with those roles can now use their respective commands.")
                    .SendToAsync(Context.Channel);

            }, false);
        }
    }
}