using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("BlacklistAdd", "BlAdd")]
        [Description("Adds a given word/phrase to the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistadd {phrase}")]
        [RequireGuildAdmin]
        public async Task BlacklistAddAsync([Remainder] string phrase)
        {
            var data = Db.GetData(Context.Guild);
            data.Configuration.Moderation.Blacklist.Add(phrase);
            Db.UpdateData(data);
            await Context.CreateEmbed($"Added **{phrase}** to the blacklist.").SendToAsync(Context.Channel);
        }

        [Command("BlacklistRemove", "BlRem")]
        [Description("Removes a given word/phrase from the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistremove {phrase}")]
        [RequireGuildAdmin]
        public async Task BlacklistRemoveAsync([Remainder] string phrase)
        {
            var data = Db.GetData(Context.Guild);
            if (data.Configuration.Moderation.Blacklist.ContainsIgnoreCase(phrase))
            {
                data.Configuration.Moderation.Blacklist.Remove(phrase);
                await Context.CreateEmbed($"Removed **{phrase}** from the word blacklist.")
                    .SendToAsync(Context.Channel);
                Db.UpdateData(data);
            }
            else
            {
                await Context.CreateEmbed($"**{phrase}** doesn't exist in the blacklist.").SendToAsync(Context.Channel);
            }
        }

        [Command("BlacklistClear", "BlCl")]
        [Description("Clears the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistclear")]
        [RequireGuildAdmin]
        public async Task BlacklistClearAsync()
        {
            var data = Db.GetData(Context.Guild);
            await Context
                .CreateEmbed(
                    $"Cleared the this guild's blacklist, containing **{data.Configuration.Moderation.Blacklist.Count}** words.")
                .SendToAsync(Context.Channel);
            data.Configuration.Moderation.Blacklist.Clear();
            Db.UpdateData(data);
        }
    }
}