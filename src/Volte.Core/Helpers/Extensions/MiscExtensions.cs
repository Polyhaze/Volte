using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using JetBrains.Annotations;
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

        public static Task PerformAsync(this BlacklistAction action, VolteContext ctx, DiscordMember member, string word)
        {
            return action switch
            {
                BlacklistAction.Warn => member.WarnAsync(ctx, $"Used blacklisted word \"{word}\"."),
                BlacklistAction.Kick => member.RemoveAsync($"Used blacklisted word \"{word}\"."),
                BlacklistAction.Ban => member.BanAsync(7, $"Used blacklisted word \"{word}\"."),
                _ => Task.CompletedTask
            };
        }
        
        public static async Task WarnAsync(this DiscordMember member, VolteContext ctx, string reason)
        {
            await ModerationModule.WarnAsync(ctx.Member, member, ctx.ServiceProvider.GetRequiredService<DatabaseService>(), ctx.ServiceProvider.GetRequiredService<LoggingService>(), reason);
        }

        [NotNull]
        public static GuildUserData GetUserData(this GuildData data, ulong id)
        {
            GuildUserData Create()
            {
                var d = new GuildUserData
                {
                    Id = id,
                    Note = string.Empty
                };
                data.UserData.Add(d);
                return d;
            }

            return data.UserData.FirstOrDefault(x => x.Id == id) ?? Create();
        }

        public static bool IsValid(this BlacklistAction current, out BlacklistAction action)
        {
            action = current;
            return current is not BlacklistAction.Nothing;
        }
    }
}