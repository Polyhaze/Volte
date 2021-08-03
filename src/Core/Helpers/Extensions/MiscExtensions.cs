using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Volte.Commands;
using Volte.Commands.Modules;
using Volte.Core.Entities;
using Volte.Core.Helpers;
using Volte.Services;
using Color = Discord.Color;

namespace Gommon
{
    public partial class Extensions
    {
        public static MemoryStream CreateColorImage(this Rgba32 color) => new MemoryStream().Apply(ms =>
        {
            using var image = new Image<Rgba32>(125, 200);
            image.Mutate(a => a.BackgroundColor(color));
            image.SaveAsPng(ms);
            ms.Position = 0;
        });

        public static Rgba32 ToRgba32(this Color color) => new Rgba32(color.R, color.G, color.B);

        public static Task PerformAsync(this BlacklistAction action, VolteContext ctx, SocketGuildUser member,
            string word) => action switch
        {
            BlacklistAction.Warn => member.WarnAsync(ctx, $"Used blacklisted phrase \"{word}\""),
            BlacklistAction.Kick => member.KickAsync($"Used blacklisted phrase \"{word}\""),
            BlacklistAction.Ban => member.BanAsync(7, $"Used blacklisted phrase \"{word}\""),
            BlacklistAction.Nothing => Task.Run(() => Logger.Debug(LogSource.Service,
                $"Guild {member.Guild} had BlacklistAction set to {nameof(BlacklistAction.Nothing)}.")),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };

        public static Task WarnAsync(this SocketGuildUser member, VolteContext ctx, string reason)
            => ModerationModule.WarnAsync(ctx.User, ctx.GuildData, member,
                ctx.Services.GetRequiredService<DatabaseService>(), reason);

        public static T ValueLock<T>(this object @lock, Func<T> action)
        {
            lock (@lock)
                return action();
        }

        public static void Lock(this object @lock, Action action)
        {
            lock (@lock)
                action();
        }

        public static void LockedRef<T>(this T obj, Action<T> action)
        {
            lock (obj)
                action(obj);
        }
    }
}