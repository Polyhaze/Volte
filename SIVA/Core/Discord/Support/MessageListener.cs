using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Objects;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord.Support
{
    public class SupportMessageListener
    {
        public static async Task Check(SocketMessage s)
        {
            var msg = (SocketUserMessage)s;
            var ctx = new SocketCommandContext(DiscordLogin.Client, msg);
            var config = ServerConfig.Get(ctx.Guild);

            if (ctx.Channel.Name.Equals(config.SupportChannelName))
            {
                await CreateSupportChannel(ctx, config);
                await msg.DeleteAsync();
            }
            
        }

        public static async Task CreateSupportChannel(SocketCommandContext ctx, Server config)
        {
            SocketRole supportRole = ctx.Guild.Roles.FirstOrDefault(r => r.Name == config.SupportRole);
            var channel = await ctx.Guild.CreateTextChannelAsync($"{config.SupportChannelName}-{ctx.User.Id}");
            await channel.ModifyAsync(x => x.CategoryId = config.SupportCategoryId);
            
            await channel.AddPermissionOverwriteAsync(
                ctx.User,
                new 
                    OverwritePermissions(
                    readMessages: PermValue.Allow, 
                    sendMessages: PermValue.Allow,
                    addReactions: PermValue.Allow, 
                    sendTTSMessages: PermValue.Deny
                    )
                );
            await channel.AddPermissionOverwriteAsync(
                ctx.Guild.EveryoneRole,
                new 
                    OverwritePermissions(
                    readMessages: PermValue.Deny, 
                    sendMessages: PermValue.Deny
                    )
                );
            await channel.AddPermissionOverwriteAsync(
                supportRole,
                new 
                    OverwritePermissions(
                    readMessages: PermValue.Allow,
                    sendMessages: PermValue.Allow,
                    addReactions: PermValue.Allow,
                    sendTTSMessages: PermValue.Deny
                    )
                );
            
            await TicketHandler.OnTicketCreation(ctx, channel, config);

        }
    }
}