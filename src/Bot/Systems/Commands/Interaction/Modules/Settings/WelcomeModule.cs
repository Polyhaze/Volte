using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionSettingsModule
{
    public partial class WelcomeModule
    {
        public WelcomeService Welcome { get; set; }

        [SlashCommand("channel", "Sets the channel used for welcoming new users for this guild.")]
        public Task<RuntimeResult> ChannelAsync(
            [Summary(description: "The channel to use for welcoming messages.")]
            ITextChannel channel)
        {
            ModifyData(d => d.Configuration.Welcome.WelcomeChannel = channel.Id);
            return Ok($"Set this guild's welcome channel to {channel.Mention}.", ephemeral: true);
        }
        
        [SlashCommand("disable", "Disable the welcome system in your guild. Set a channel to re-enable.")]
        public Task<RuntimeResult> DisableAsync()
        {
            ModifyData(d => d.Configuration.Welcome.WelcomeChannel = 0);
            return Ok("Disabled the welcoming system in this guild.", ephemeral: true);
        }

        [SlashCommand("color", "Sets the color used for welcome embeds for this guild.")]
        public Task<RuntimeResult> ColorAsync(
            [Summary(description: "Hexadecimal number (with/without #) or RGB number separated by ,")] 
            Color color)
        {
            ModifyData(d => d.Configuration.Welcome.WelcomeColor = color.RawValue);
            return Ok("Successfully set this guild's welcome message embed color!", ephemeral: true);
        }

        [SlashCommand("left", "Sets or shows the leaving message used to say bye for this guild.")]
        public Task<RuntimeResult> LeftAsync(
            [Autocomplete<WelcomeStarscriptAutocompleter>]
            [Summary(description: "The message to be displayed when a user leaves your server. Powered by Starscript.")]
            string message = null)
        {
            if (message is null)
                return Ok(
                    $"The current leaving message for this guild is: {Format.Code(GetData().Configuration.Welcome.LeavingMessage ?? "None", string.Empty)}",
                    ephemeral: true);

            ModifyData(data => data.Configuration.Welcome.LeavingMessage = message);

            var welcomeChannel = Context.Guild.GetTextChannel(GetData().Configuration.Welcome.WelcomeChannel);
            var sendingTest = welcomeChannel is null
                ? "Not sending a test message, as you do not have a welcome channel set. " +
                  "Set a welcome channel to fully complete the setup!"
                : $"Sending a test message to {welcomeChannel.Mention}.";

            return Ok(new StringBuilder()
                    .AppendLine($"Set this server's leaving message to: {Format.Code(message, string.Empty)}")
                    .AppendLine()
                    .AppendLine($"{sendingTest}"),
                () => Welcome.LeaveAsync(new UserLeftEventArgs(Context.Guild, Context.User)),
                ephemeral: true);
        }

        [SlashCommand("join", "Sets or shows the message used to welcome new users to this guild.")]
        public Task<RuntimeResult> JoinAsync(
            [Autocomplete<WelcomeStarscriptAutocompleter>]
            [Summary(description: "The message to be displayed when a user joins your server. Powered by Starscript.")]
            string message = null)
        {
            if (message is null)
                return Ok(
                    $"The current joining message for this guild is: {Format.Code(GetData().Configuration.Welcome.WelcomeMessage ?? "None", string.Empty)}",
                    ephemeral: true);

            ModifyData(data => data.Configuration.Welcome.WelcomeMessage = message);

            var welcomeChannel = Context.Guild.GetTextChannel(GetData().Configuration.Welcome.WelcomeChannel);
            var sendingTest = welcomeChannel is null
                ? "Not sending a test message, as you do not have a welcome channel set. " +
                  "Set a welcome channel to fully complete the setup!"
                : $"Sending a test message to {welcomeChannel.Mention}.";

            return Ok(new StringBuilder()
                    .AppendLine($"Set this server's joining message to: {Format.Code(message, string.Empty)}")
                    .AppendLine()
                    .AppendLine($"{sendingTest}"),
                () => Welcome.JoinAsync(
                    new UserJoinedEventArgs(
                        Context.User.Cast<SocketGuildUser>() ?? Context.Guild.GetUser(Context.User.Id)
                    )
                ),
                ephemeral: true);
        }

        [SlashCommand("dm", "Sets or shows the message to be (attempted to) sent to members upon joining.")]
        public Task<RuntimeResult> DmAsync(
            [Autocomplete<WelcomeStarscriptAutocompleter>]
            [Summary(
                description:
                "The message you want to sent. Use 'disable' to disable welcome DMs. Powered by Starscript.")]
            string message = null)
        {
            if (message is null)
                return Ok(
                    $"The current welcome DM for this guild is: {Format.Code(GetData().Configuration.Welcome.WelcomeDmMessage ?? "None", string.Empty)}",
                    ephemeral: true);

            if (message is "disable")
            {
                ModifyData(data => data.Configuration.Welcome.WelcomeDmMessage = string.Empty);
                return Ok("Disabled welcome DMs.", ephemeral: true);
            }

            ModifyData(data => data.Configuration.Welcome.WelcomeDmMessage = message);

            return Ok(new StringBuilder()
                    .AppendLine($"Set this server's welcome DM to: {Format.Code(message, string.Empty)}")
                    .AppendLine()
                    .AppendLine("Trying to send a test message to your DM."),
                () => Welcome.DmAsync(
                    new UserJoinedEventArgs(
                        Context.User.Cast<SocketGuildUser>() ?? Context.Guild.GetUser(Context.User.Id)
                    )
                ),
                ephemeral: true);
        }
    }
}