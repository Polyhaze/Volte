using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BrackeysBot.Commands
{
    public sealed partial class ModerationModule : BrackeysBotModule
    {
        [Command("mute")]
        [Summary("Mutes a member, with an optional reason and duration.")]
        [Remarks("mute <user> [duration] [reason]")]
        [Priority(1)]
        [RequireModerator]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task MuteAsync(
            [Summary("The user to mute.")] SocketGuildUser user,
            [Summary("The duration for the mute."), OverrideTypeReader(typeof(AbbreviatedTimeSpanTypeReader))] TimeSpan duration,
            [Summary("The reason why to mute the user."), Remainder] string reason = DefaultReason)
            => await TempmuteAsync(user, duration, reason);

        [Command("mute")]
        [Summary("Mutes a member, with an optional reason.")]
        [Remarks("mute <user> [reason]")]
        [HideFromHelp]
        [RequireModerator]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task MuteAsync(
            [Summary("The user to mute.")] SocketGuildUser user,
            [Summary("The reason why to mute the user."), Remainder] string reason = DefaultReason)
        {
            await user.MuteAsync(Context);

            SetUserMuted(user.Id, true);

            Moderation.AddInfraction(user, Infraction.Create(Moderation.RequestInfractionID())
                .WithType(InfractionType.Mute)
                .WithModerator(Context.User)
                .WithDescription(reason));
            
            await ModerationLog.CreateEntry(ModerationLogEntry.New
                .WithDefaultsFromContext(Context)
                .WithActionType(ModerationActionType.Mute)
                .WithTarget(user)
                .WithReason(reason), Context.Channel);
        }

        [Command("tempmute")]
        [Summary("Temporarily mutes a member for the specified duration, for the optional reason.")]
        [Remarks("tempmute <user> <duration> [reason]")]
        [Priority(1)]
        [HideFromHelp]
        [RequireModerator]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task TempmuteAsync(
            [Summary("The user to mute.")] SocketGuildUser user,
            [Summary("The duration for the mute."), OverrideTypeReader(typeof(AbbreviatedTimeSpanTypeReader))] TimeSpan duration,
            [Summary("The reason why to mute the user."), Remainder] string reason = DefaultReason)
        {
            await user.MuteAsync(Context);

            SetUserMuted(user.Id, true);

            Moderation.AddTemporaryInfraction(TemporaryInfractionType.TempMute, user, Context.User, duration, reason);
            
            await ModerationLog.CreateEntry(ModerationLogEntry.New
                .WithDefaultsFromContext(Context)
                .WithActionType(ModerationActionType.TempMute)
                .WithTarget(user)
                .WithDuration(duration)
                .WithReason(reason), Context.Channel);
        }

        [Command("unmute")]
        [Summary("Unmutes a user.")]
        [Remarks("unmute <user>")]
        [RequireModerator]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnmuteAsync(
            [Summary("The user to unmute.")] SocketGuildUser user)
        {
            await user.UnmuteAsync(Context);

            SetUserMuted(user.Id, false);

            Moderation.ClearTemporaryInfraction(TemporaryInfractionType.TempMute, user);
            
            await ModerationLog.CreateEntry(ModerationLogEntry.New
                .WithDefaultsFromContext(Context)
                .WithActionType(ModerationActionType.Unmute)
                .WithTarget(user), Context.Channel);
        }

        private void SetUserMuted(ulong id, bool muted) {
            Data.UserData.GetOrCreate(id).Muted = muted;
            Data.SaveUserData();
        }
    }
}
