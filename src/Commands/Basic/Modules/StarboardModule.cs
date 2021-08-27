using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Commands.Modules
{
    [Group("Starboard", "Sb")]
    [RequireGuildAdmin]
    public sealed class StarboardModule : VolteModule
    {
        public StarboardService Service { get; set; }
        
        [Command, DummyCommand, Description("The set of commands used to modify how the Starboard works in your guild.")]
        public Task<ActionResult> BaseAsync() => None();
        
        [Command("Channel", "Ch")]
        [Description("Sets the channel to be used by starboard when a message is starred.")]
        public Task<ActionResult> ChannelAsync(
            [Description("The channel to be used by Starboard.")] SocketTextChannel channel)
        {
            Context.Modify(data => data.Configuration.Starboard.StarboardChannel = channel.Id);
            return Ok($"Successfully set the starboard channel to {MentionUtils.MentionChannel(channel.Id)}.");
        }

        [Command("Amount", "Count")]
        [Description("Sets the amount of stars required on a message for it to be posted to the Starboard.")]
        public Task<ActionResult> AmountAsync(
            [Description("The desired star count threshold before posting it in the starboard channel.")] int amount)
        {
            if (amount < 1)
                return BadRequest("Amount must be larger than zero.");
            

            Context.Modify(data => data.Configuration.Starboard.StarsRequiredToPost = amount);
            
            return Ok($"Set the amount of stars required to be posted as a starboard message to **{amount}**.");
        }

        [Command("Setup")]
        [Description("A one-off command that creates a channel for Starboard, with read-only permissions for everyone, and enables the starboard.")]
        public async Task<ActionResult> SetupAsync(
            [Description("The name for the Starboard channel that will be created."), Remainder] string channelName = "starboard")
        {
            var channel = await Context.Guild.CreateTextChannelAsync(channelName.Replace(" ", "-"), props =>
            {
                props.CategoryId = Context.Channel.CategoryId;
                props.PermissionOverwrites = new List<Overwrite>
                {
                    new Overwrite(Context.Guild.EveryoneRole.Id, PermissionTarget.Role, 
                        new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny))
                };
            });
            
            Context.Modify(data =>
            {
                data.Configuration.Starboard.Enabled = true;
                data.Configuration.Starboard.StarboardChannel = channel.Id;
            });
            
            return Ok($"Successfully configured the Starboard functionality, and any starred messages will go to {channel.Mention}.");
        }

        [Command("Enable")]
        [Description("Enable or disable the Starboard in this guild.")]
        public Task<ActionResult> EnableAsync(
            [Description("Whether or not to enable or disable the Starboard.")] bool enabled)
        {
            Context.Modify(data => data.Configuration.Starboard.Enabled = enabled);
            return Ok(
                enabled ? "Enabled the Starboard in this Guild." : "Disabled the Starboard in this Guild.");
        }
    }
}