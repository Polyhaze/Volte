using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Data;
using Volte.Data.Models.EventArgs;
using Volte.Services;

namespace Gommon
{
    public static partial class Extensions
    {
        public static string GetInviteUrl(this IDiscordClient client, bool shouldHaveAdmin = true)
        {
            return shouldHaveAdmin
                ? $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=8"
                : $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=0";
        }

        public static IUser GetOwner(this DiscordShardedClient client) => client.GetUser(Config.Owner);

        public static IGuild GetPrimaryGuild(this DiscordShardedClient client) => client.GetGuild(405806471578648588);

        public static Task RegisterVolteEventHandlersAsync(this DiscordShardedClient client, ServiceProvider provider)
        {
            var welcome = provider.GetRequiredService<WelcomeService>();
            var logger = provider.GetRequiredService<LoggingService>();
            var guild = provider.GetRequiredService<GuildService>();
            var @event = provider.GetRequiredService<EventService>();
            var autorole = provider.GetRequiredService<AutoroleService>();
            return Task.Run(() =>
            {
                client.Log += m => logger.Log(new LogEventArgs(m));
                client.JoinedGuild += g => guild.OnJoinAsync(new JoinedGuildEventArgs(g));
                client.LeftGuild += g => guild.OnLeaveAsync(new LeftGuildEventArgs(g));
                client.UserJoined += user =>
                {
                    if (Config.EnabledFeatures.Welcome)
                        return welcome.JoinAsync(new UserJoinedEventArgs(user));
                    if (Config.EnabledFeatures.Autorole)
                        return autorole.ApplyRoleAsync(new UserJoinedEventArgs(user));
                    return Task.CompletedTask;
                };
                client.UserLeft += user =>
                {
                    if (Config.EnabledFeatures.Welcome)
                        return welcome.LeaveAsync(new UserLeftEventArgs(user));
                    return Task.CompletedTask;
                };
                client.ShardReady += (c) => @event.OnReady(new ReadyEventArgs(c));
                client.MessageReceived += async (s) =>
                {
                    if (!(s is IUserMessage msg)) return;
                    if (msg.Author.IsBot) return;
                    if (msg.Channel is IDMChannel)
                    {
                        await msg.Channel.SendMessageAsync("Currently, I do not support commands via DM.");
                        return;
                    }

                    await @event.HandleMessageAsync(new MessageReceivedEventArgs(s, provider));
                };
            });
        }
    }
}