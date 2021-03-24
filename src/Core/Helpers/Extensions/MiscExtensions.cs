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
using Volte.Services;

namespace Gommon
{
    public partial class Extensions
    {
        
        public static MemoryStream CreateColorImage(this Rgba32 color)
        {
            var @out = new MemoryStream();
            using var image = new Image<Rgba32>(200, 200);
            image.Mutate(a => a.BackgroundColor(color));
            image.SaveAsPng(@out);
            @out.Position = 0;
            return @out;
        }

        public static async Task PerformAsync(this BlacklistAction action, VolteContext ctx, SocketGuildUser member, string word, ModerationService mod)
        {
            switch (action)
            {
                case BlacklistAction.Warn:
                    await member.WarnAsync(ctx, $"Said blacklisted word \"{word}\"");
                    break;
                case BlacklistAction.Kick:
                    await member.KickAsync($"Said blacklisted word \"{word}\"");
                    break;
                case BlacklistAction.Ban:
                    await member.BanAsync(7, $"Said blacklisted word \"{word}\"");
                    break;
                case BlacklistAction.Nothing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        
        public static async Task WarnAsync(this SocketGuildUser member, VolteContext ctx, string reason)
        {
            await ModerationModule.WarnAsync(ctx.User, ctx.GuildData, member, ctx.Services.GetRequiredService<DatabaseService>(), reason);
        }
        
    }
}