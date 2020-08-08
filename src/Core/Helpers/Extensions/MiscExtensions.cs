using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Volte.Commands;
using Volte.Commands.Modules;
using Volte.Core.Models.Guild;
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

        public static async Task PerformAsync(this BlacklistAction action, VolteContext ctx, SocketGuildUser member, string word)
        {
            switch (action)
            {
                case BlacklistAction.Warn:
                    await member.WarnAsync(ctx, $"Used blacklisted word \"{word}\".");
                    break;
                case BlacklistAction.Kick:
                    await member.KickAsync($"Used blacklisted word \"{word}\".");
                    break;
                case BlacklistAction.Ban:
                    await member.BanAsync(7, $"Used blacklisted word \"{word}\".");
                    break;
                case BlacklistAction.Nothing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        
        public static async Task WarnAsync(this SocketGuildUser member, VolteContext ctx, string reason)
        {
            await ModerationModule.WarnAsync(ctx.User, ctx.GuildData, member, ctx.ServiceProvider.GetRequiredService<DatabaseService>(), ctx.ServiceProvider.GetRequiredService<LoggingService>(), reason);
        }

        public static GuildUserData GetUserData(this GuildData data, ulong id)
        {
            GuildUserData Create()
            {
                var d = new GuildUserData
                {
                    Id = id
                };
                data.UserData.Add(d);
                return d;
            }

            return data.UserData.FirstOrDefault(x => x.Id == id) ?? Create();

        }
        
    }
}