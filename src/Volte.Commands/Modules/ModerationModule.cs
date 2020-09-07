using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;
using Volte.Core;
using Volte.Core.Helpers;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Commands.Modules
{
    [RequireGuildModerator]
    public sealed class ModerationModule : VolteModule 
    {
        public static async Task WarnAsync(
            DiscordMember issuer, 
            GuildData data, 
            DiscordMember member, 
            DatabaseService db, 
            LoggingService logger, 
            string reason)
        {
            data.Extras.Warns.Add(new Warn
            {
                User = member.Id,
                Reason = reason,
                Issuer = issuer.Id,
                Date = DateTimeOffset.Now
            });
            db.UpdateData(data);
            var embed = new DiscordEmbedBuilder()
                .WithColor(member.GetHighestRoleWithColor()?.Color ?? new DiscordColor(Config.SuccessColor))
                .WithAuthor(issuer)
                .WithDescription($"You've been warned in **{issuer.Guild.Name}** for **{reason}**.")
                .Build();

            if (!await member.TrySendMessageAsync(
                embed: embed))
            {
                logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }
        }

        [Group("Warn", "Warns")]
        public sealed class WarnsModule : VolteModule
        {
            [Command]
            [Description("Warns the target user for the given reason.")]
            [Priority(100)]
            [Remarks("warn {Member} {String}")]
            public async Task<ActionResult> WarnAsync([CheckHierarchy] DiscordMember user, [Remainder] string reason)
            {
                await ModerationModule.WarnAsync(Context.Member, Context.GuildData, user, Db, Logger, reason);

                return Ok($"Successfully warned **{user}** for **{reason}**.",
                    _ => ModLogService.DoAsync(ModActionEventArgs.New
                        .WithDefaultsFromContext(Context)
                        .WithActionType(ModActionType.Warn)
                        .WithTarget(user)
                        .WithReason(reason))
                );
            }

            [Command("List", "L")]
            [Description("Shows all the warns for the given user.")]
            [Remarks("warn list {Member}")]
            public Task<ActionResult> WarnsAsync(DiscordMember user)
            {
                var warns = Context.GuildData.Extras.Warns.Where(x => x.User == user.Id).ToList();
                if (warns.IsEmpty()) return BadRequest("This user doesn't have any warnings.");
                else
                {
                    return None(async () =>
                    {
                        await Context.Interactivity.SendPaginatedMessageAsync(Context.Channel, Context.Member,
                            warns.Select(x => $"**{x.Reason}**, on **{x.Date.FormatDate()}**").GetPages(10));
                    }, false);
                }
            }

            [Command("Clear", "C")]
            [Description("Clears the warnings for the given user.")]
            [Remarks("warn clear {Member}")]
            public async Task<ActionResult> ClearWarnsAsync(DiscordMember user)
            {
                var oldWarnList = Context.GuildData.Extras.Warns;
                var newWarnList = Context.GuildData.Extras.Warns.Where(x => x.User != user.Id).ToList();
                ModifyData(data =>
                {
                    data.Extras.Warns = newWarnList;
                    return data;
                });

                try
                {
                    await Context.CreateEmbed($"Your warns in **{Context.Guild.Name}** have been cleared. Hooray!")
                        .SendToAsync(user);
                }
                catch (UnauthorizedException e)
                {
                    Logger.Warn(LogSource.Volte,
                        $"encountered a 403 when trying to message {user}!", e);
                }

                return Ok($"Cleared **{oldWarnList.Count - newWarnList.Count}** warnings for **{user}**.", _ =>
                    ModLogService.DoAsync(ModActionEventArgs.New
                        .WithDefaultsFromContext(Context)
                        .WithActionType(ModActionType.ClearWarns)
                        .WithTarget(user))
                );
            }
        }

        [Command("Softban")]
        [Description("Softbans the mentioned user, kicking them and deleting the last x (where x is defined by the daysToDelete parameter) days of messages.")]
        [Remarks("softban {Member} [Int] [String]")]
        [RequireBotGuildPermission(Permissions.KickMembers | Permissions.BanMembers)]
        public async Task<ActionResult> SoftBanAsync([CheckHierarchy] DiscordMember user, int daysToDelete = 0,
            [Remainder] string reason = "Softbanned by a Moderator.")
        {
            if (!await user.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been softbanned from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {user}!");
            }

            await user.BanAsync(daysToDelete == 0 ? 7 : daysToDelete, reason);
            await Context.Guild.UnbanMemberAsync(user);

            return Ok($"Successfully softbanned **{user.Username}#{user.Discriminator}**.", _ =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithTarget(user)
                    .WithReason(reason))
            );
        }

        [Command("Purge", "clear", "clean")]
        [Description("Purges the last x messages, or the last x messages by a given user.")]
        [Remarks("purge {Int} [RestUser]")]
        [RequireBotChannelPermission(Permissions.ManageMessages)]
        public async Task<ActionResult> PurgeAsync(int count, DiscordUser targetAuthor = null)
        {
            //+1 to include the command invocation message, and actually delete the last x messages instead of x - 1.
            //lets you theoretically use 0 to delete only the invocation message, for testing or something.
            var messages = (await Context.Channel.GetMessagesAsync(count + 1)).ToList();
            try
            {
                if (!(targetAuthor is null))
                    await Context.Channel.DeleteMessagesAsync(messages.Where(x => x.Author.Id == targetAuthor.Id));
                else
                    await Context.Channel.DeleteMessagesAsync(messages);

            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest(
                    "Messages bulk deleted must be younger than 14 days. (**This is a Discord restriction, not a Volte one.**)");
            }

            //-1 to show that the correct amount of messages were deleted.
            return Ok($"Successfully deleted **{"message".ToQuantity(messages.Count - 1)}**", m =>
            {
                _ = Executor.ExecuteAfterDelayAsync(TimeSpan.FromSeconds(3), async () => await m.TryDeleteAsync());
                return ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Purge)
                    .WithCount(count));
            });
        }

        [Command("Note")]
        [Description("Adds a note to a user to let other moderators know something relevant about them.")]
        [Remarks("note {Member} [String]")]
        public Task<ActionResult> NoteAsync(DiscordMember user, [Remainder] string note = null)
        {
            var userNote = Context.GuildData.GetUserData(user.Id).Note;
            if (note is null)
            {
                return Ok(userNote.IsNullOrEmpty() ? "No note provided." : userNote);
            }
            
            Context.GuildData.GetUserData(user.Id).Note = note;
            Db.UpdateData(Context.GuildData);

            return userNote.IsNullOrEmpty()
                ? Ok($"Successfully set **{user}**'s note to `{note}`")
                : Ok($"Changed **{user}**'s note from `{userNote}` to `{note}`");
        }

        [Command("ModProfile", "MP")]
        [Description("Shows a moderator relevant information about a user, or if no user is given, yourself.")]
        [Remarks("modprofile [Member]")]
        public Task<ActionResult> ModProfileAsync(DiscordMember user = null)
        {
            user ??= Context.Member;
            var ud = Context.GuildData.GetUserData(user.Id);

            var note = ud.Note.IsNullOrEmpty() ? "No note provided." : ud.Note;
            var e = Context.CreateEmbedBuilder()
                .WithAuthor($"{user}'s Moderator Profile", user.GetAvatarUrl(ImageFormat.Auto, 256))
                .WithThumbnail(user.GetAvatarUrl(ImageFormat.Auto, 512))
                .AddField("Username/Nickname", user.DisplayName, true)
                .AddField("Discriminator", user.Discriminator, true)
                .AddField("Can use Volte Mod Commands", user.IsModerator(Context), true)
                .AddField("Has ever been Kicked/Banned", ud.Actions.Any(x
                    => x.Type is ModActionType.Ban || x.Type is ModActionType.Kick || 
                       x.Type is ModActionType.Softban || x.Type is ModActionType.IdBan), true)
                .AddField("# of Warns", Context.GuildData.Extras.Warns.Count(x => x.User == user.Id), true)
                .AddField("Note", $"`{note}`", true)
                .WithFooter("Please note that the numbers shown above ONLY track bans/kicks done via Volte; if you've been banned manually it won't show up here.");

            return Ok(e);

        }

        [Command("Kick")]
        [Description("Kicks the given user.")]
        [Remarks("kick {Member} [String]")]
        [RequireBotGuildPermission(Permissions.KickMembers)]
        public async Task<ActionResult> KickAsync([CheckHierarchy] DiscordMember user,
            [Remainder] string reason = "Kicked by a Moderator.")
        {
            if (!await user.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been kicked from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {user}!");
            }

            await user.RemoveAsync(reason);

            return Ok($"Successfully kicked **{user.Username}#{user.Discriminator}** from this guild.", m =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Kick)
                    .WithTarget(user)
                    .WithReason(reason))
            );
        }

        [Command("IdBan")]
        [Description("Bans a user based on their ID.")]
        [Remarks("idban {Ulong} [String]")]
        [RequireBotGuildPermission(Permissions.BanMembers)]
        public async Task<ActionResult> IdBanAsync(ulong user,
            [Remainder] string reason = "Banned by a Moderator.")
        {
            await Context.Guild.BanMemberAsync(user, 0, reason);
            return Ok("Successfully banned that user from this guild.", async m => 
                await ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.IdBan)
                    .WithTarget(user)
                    .WithReason(reason))
            );
        }

        [Command("Delete")]
        [Description("Deletes a message in the current channel by its ID. Creates an audit log entry for abuse prevention.")]
        [Remarks("delete {Ulong}")]
        [RequireBotChannelPermission(Permissions.ManageMessages)]
        public async Task<ActionResult> DeleteAsync(ulong messageId)
        {
            var target = await Context.Channel.GetMessageAsync(messageId);
            if (target is null)
                return BadRequest("That message doesn't exist in this channel.");

            await target.TryDeleteAsync($"Message deleted by Moderator {Context.Member}.");

            return Ok($"{EmojiHelper.BallotBoxWithCheck} Deleted that message.", async m =>
            {
                _ = Executor.ExecuteAfterDelayAsync(TimeSpan.FromSeconds(3), async () =>
                {
                    await Context.Message.TryDeleteAsync();
                    await m.DeleteAsync();
                });

                await ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Delete)
                    .WithTarget(messageId)
                );
            });
        }

        [Command("Bans")]
        [Description("Shows all bans in this guild.")]
        [Remarks("bans")]
        [RequireBotGuildPermission(Permissions.BanMembers)]
        public async Task<ActionResult> BansAsync()
        {
            var banList = await Context.Guild.GetBansAsync();
            if (banList.IsEmpty()) return BadRequest("This guild doesn't have anyone banned.");
            else
            {
                return None(async () =>
                {
                    await Context.Interactivity.SendPaginatedMessageAsync(Context.Channel, Context.Member,
                        banList.Select(x => $"**{x.User}**: `{x.Reason ?? "No reason provided."}`").GetPages(10));
                }, false);
            }
        }

        [Command("ShadowBan", "Shdwb")]
        [Description("Shadowbans the mentioned user, completely skipping sending a message to the modlog.")]
        [Remarks("shadowban {Member} [Reason]")]
        [RequireBotGuildPermission(Permissions.BanMembers)]
        public async Task<ActionResult> ShadowBanAsync([CheckHierarchy] DiscordMember member, 
            [Remainder] string reason = "Shadowbanned by a Moderator.")
        {
            if (!await member.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been shadowbanned from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }
            
            await member.BanAsync(7, reason);
            var ud = Context.GuildData.GetUserData(member.Id);
            ud.Actions.Add(new ModAction
            {
                Moderator = Context.Member.Id,
                Reason = reason,
                Time = Context.Now,
                Type = ModActionType.Ban
            });
            return Ok($"Successfully shadowbanned **{member.Mention}** from this guild.");
        }

        [Command("Ban")]
        [Description("Bans the mentioned user.")]
        [Remarks("ban {Member} [String]")]
        [RequireBotGuildPermission(Permissions.BanMembers)]
        public async Task<ActionResult> BanAsync([CheckHierarchy] DiscordMember user,
            [Remainder] string reason = "Banned by a Moderator.")
        {
            if (!await user.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been banned from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {user}!");
            }

            await user.BanAsync(7, reason);
            return Ok($"Successfully banned **{user.Mention}** from this guild.", _ =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Ban)
                    .WithTarget(user)
                    .WithReason(reason))
            );
        }

        [Command("Actions")]
        [Description("Show all the moderator actions taken against a user.")]
        [Remarks("actions [User]")]
        public async Task<ActionResult> ActionsAsync([Remainder]DiscordMember user = null)
        {
            user ??= Context.Member;
            var allActions = Context.GuildData.GetUserData(user.Id).Actions;
            var l = new List<string>();

            foreach (var action in allActions)
            {
                var mod = await Context.Client.ShardClients.First().Value.GetUserAsync(action.Moderator);
                var reason = action.Reason.IsNullOrEmpty() ? "No reason provided." : action.Reason;
                var str = action.Type switch
                {
                    ModActionType.Ban => $"**Ban**: {action.Time.FormatDate()}, for `{reason}` by **{mod}**.",
                    ModActionType.ClearWarns => $"**Cleared Warnings**: {action.Time.FormatDate()}, by **{mod}**.",
                    ModActionType.IdBan => $"**Id Ban**: {action.Time.FormatDate()}, for `{ reason}` by **{mod}**.",
                    ModActionType.Kick => $"**Kick**: {action.Time.FormatDate()}, for `{reason}` by **{mod}**.",
                    ModActionType.Softban => $"**Softban**: {action.Time.FormatDate()}, for `{reason}` by **{mod}**.",
                    ModActionType.Warn => $"**Warn**: {action.Time.FormatDate()}, for `{reason}` by **{mod}**.",
                    _ => ""
                };

                l.Add(str);
            }

            return None(async () =>
            {
                await Context.Interactivity.SendPaginatedMessageAsync(Context.Channel, Context.Member, l.GetPages(10));
            }, false);
        }
    }
}