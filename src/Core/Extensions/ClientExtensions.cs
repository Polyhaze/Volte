using System.Threading.Tasks;
using Discord;
 
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core;
using Volte.Core.Models.EventArgs;
using Volte.Services;

namespace Gommon
{
    public static partial class Extensions
    {
        public static string GetInviteUrl(this IDiscordClient client, bool withAdmin = true)
        {
            return withAdmin
                ? $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=8"
                : $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=402992246";
        }

        public static SocketUser GetOwner(this BaseSocketClient client)
        {
            return client.GetUser(Config.Owner);
        }

        public static SocketGuild GetPrimaryGuild(this BaseSocketClient client)
        {
            return client.GetGuild(405806471578648588);
        }

        public static Task RegisterVolteEventHandlersAsync(this DiscordShardedClient client, ServiceProvider provider)
        {
            provider.Get<WelcomeService>(out var welcome);
            provider.Get<GuildService>(out var guild);
            provider.Get<EventService>(out var evt);
            provider.Get<AutoroleService>(out var autorole);
            provider.Get<LoggingService>(out var logger);
            return Executor.ExecuteAsync(() =>
            {
                client.Log += m => logger.DoAsync(new LogEventArgs(m));
                client.JoinedGuild += g => guild.OnJoinAsync(new JoinedGuildEventArgs(g));
                client.LeftGuild += g => guild.OnLeaveAsync(new LeftGuildEventArgs(g));
                client.UserJoined += user =>
                {
                    if (Config.EnabledFeatures.Welcome)
                        return welcome.JoinAsync(new UserJoinedEventArgs(user));
                    if (Config.EnabledFeatures.Autorole)
                        return autorole.DoAsync(new UserJoinedEventArgs(user));
                    return Task.CompletedTask;
                };
                client.UserLeft += user =>
                    Config.EnabledFeatures.Welcome
                        ? welcome.LeaveAsync(new UserLeftEventArgs(user))
                        : Task.CompletedTask;
                client.ShardReady += c => evt.OnReady(new ReadyEventArgs(c));
                client.MessageReceived += async s =>
                {
                    if (!(s is SocketUserMessage msg)) return;
                    if (msg.Author.IsBot) return;
                    if (msg.Channel is IDMChannel)
                    {
                        await msg.Channel.SendMessageAsync("Currently, I do not support commands via DM.");
                        return;
                    }

                    await evt.HandleMessageAsync(new MessageReceivedEventArgs(s, provider));
                };
                return Task.CompletedTask;
            });
        }
    }
}