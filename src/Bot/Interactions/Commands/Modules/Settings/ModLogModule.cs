using Discord.Interactions;

namespace Volte.Interactions.Commands.Modules;

public partial class InteractionSettingsModule
{
    [Discord.Interactions.Group("mod-log", "Set the mod log channel or disable it.")]
    public class ModLogModule : VolteSlashCommandModule
    {
        [SlashCommand("channel", "Sets the channel used for the mod log.")]
        public Task<RuntimeResult> ChannelAsync(
            [Summary(description: "The channel to use for the mod log.")]
            ITextChannel channel)
        {
            ModifyData(d => d.Configuration.Moderation.ModActionLogChannel = channel.Id);
            return Ok($"Set this guild's welcome channel to {channel.Mention}.", ephemeral: true);
        }
        
        [SlashCommand("disable", "Disable the mod log system in your guild. Set a channel to re-enable.")]
        public Task<RuntimeResult> DisableAsync()
        {
            ModifyData(d => d.Configuration.Moderation.ModActionLogChannel = 0);
            return Ok("Disabled the mod log system in this guild.", ephemeral: true);
        }
    }
}