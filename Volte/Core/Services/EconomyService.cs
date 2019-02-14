using System.Threading.Tasks;
using Qmmands;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Commands;
using Volte.Core.Helpers;

namespace Volte.Core.Services {
    internal class EconomyService {
        internal async Task Give(VolteContext ctx) {
            var db = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>();
            var config = db.GetConfig(ctx.Guild.Id);
            var userData = db.GetUser(ctx.User.Id);
            if (config.Leveling) {
                var oldLevel = userData.Level;
                userData.Money += 1;
                userData.Xp += 5;
                db.UpdateUser(userData);

                if (oldLevel != userData.Level) {
                    var levelUp = await ctx.Channel.SendMessageAsync(string.Empty, false,
                        Utils.CreateEmbed(ctx,
                            $"Good job {ctx.User.Mention}! You leveled up to level **{userData.Level}**!"));
                    await Task.Delay(5000);
                    await levelUp.DeleteAsync();
                }
            }
        }
    }
}