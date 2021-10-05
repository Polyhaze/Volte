using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Volte.Helpers;
using Volte.Interactions;

namespace Volte.Commands.Application
{
    public sealed class IamCommand : ApplicationCommand
    {
        public IamCommand() : base("iam", "Give yourself Self Roles via dropdown menu.", true) { }

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var roles = ctx.GuildSettings.Extras.SelfRoleIds
                .Select(x => ctx.Guild.GetRole(x))
                .WhereNotNull()
                .Where(x => !ctx.GuildUser.HasRole(x.Id))
                .ToList();

            var reply = ctx.CreateReplyBuilder(true);


            if (roles.IsEmpty())
                reply.WithEmbedFrom("This guild has no more roles available for self-assigning.");
            else
                reply.WithEmbedFrom("What roles would you like?")
                    .WithSelectMenu(new SelectMenuBuilder()
                        .WithCustomId("iam:menu")
                        .WithMaxValues(roles.Count)
                        .WithPlaceholder("Choose a role...")
                        .AddOptions(roles.Select(r => new SelectMenuOptionBuilder()
                            .WithLabel(r.Name)
                            .WithValue(r.Id.ToString()))
                        )
                    );

            await reply.RespondAsync();
        }

        public override async Task HandleComponentAsync(MessageComponentContext ctx)
        {
            if (ctx.CustomId.Split(':')[1] is "menu")
            {
                var roles = ctx.SelectedMenuOptions.Select(x => ctx.Guild.GetRole(ulong.Parse(x))).ToArray();
                await ctx.GuildUser.AddRolesAsync(roles);
                await ctx.CreateReplyBuilder(true).WithEmbed(x =>
                {
                    x.WithTitle($"You now have the following {"role".ToQuantity(roles.Length).Split(' ')[1]}");
                    x.WithDescription(roles.Select(r => r.Mention).Join(", "));
                }).RespondAsync();
            }
        }
    }

    public sealed class IamNotCommand : ApplicationCommand
    {
        public IamNotCommand() : base("iamnot", "Take away Self Roles from yourself via dropdown menu.", true) { }

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var roles = ctx.GuildSettings.Extras.SelfRoleIds
                .Select(x => ctx.Guild.GetRole(x))
                .Where(x => x != null)
                .Where(x => ctx.GuildUser.HasRole(x.Id))
                .ToList();

            var reply = ctx.CreateReplyBuilder(true);


            if (roles.IsEmpty())
                reply.WithEmbedFrom("You don't have any self roles.");
            else
                reply.WithEmbedFrom("What roles would you like taken away?")
                    .WithSelectMenu(new SelectMenuBuilder()
                        .WithCustomId("iamnot:menu")
                        .WithMaxValues(roles.Count)
                        .AddOptions(roles.Select(r => new SelectMenuOptionBuilder()
                            .WithLabel(r.Name)
                            .WithValue(r.Id.ToString()))
                        )
                    );

            await reply.RespondAsync();
        }

        public override async Task HandleComponentAsync(MessageComponentContext ctx)
        {
            await ctx.DeferAsync(true);
            if (ctx.CustomId.Split(':')[1] != "menu") return;

            await ctx.GuildUser.RemoveRolesAsync(
                ctx.SelectedMenuOptions.Select(x => ctx.Guild.GetRole(ulong.Parse(x))));
        }
    }

    public class SelfRoleCommand : ApplicationCommand
    {
        public SelfRoleCommand() : base("self-roles", "Modify the current guild's list of self roles.", true) =>
            Signature(o =>
            {
                o.Subcommand("add", "Add a role to the list of self roles for this guild.", x =>
                    x.RequiredRole("role", "The role to add to the list of self roles.")
                );
                o.Subcommand("remove", "Remove roles from the list of self roles via dropdown menu.");
            });

        private readonly Func<IEnumerable<SocketRole>, SelectMenuBuilder> _getSelfRoleRemoveMenu = rs =>
        {
            var roles = rs as SocketRole[] ?? rs.ToArray();
            return new SelectMenuBuilder()
                .WithCustomId("self-roles:remove")
                .WithMaxValues(roles.Length)
                .WithMinValues(1)
                .WithOptions(roles.Take(25)
                    .Select(r => new SelectMenuOptionBuilder()
                        .WithLabel(r.Name)
                        .WithValue(r.Id.ToString()))
                    .ToList())
                .WithPlaceholder($"Choose up to {"role".ToQuantity(roles.Length)}...");
        };

        public override Task<bool> RunSlashChecksAsync(SlashCommandContext ctx) =>
            Task.FromResult(ctx.IsAdmin(ctx.GuildUser));

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var reply = ctx.CreateReplyBuilder().WithEphemeral();
            var subcommand = ctx.Options.First().Value;

            if (!await RunSlashChecksAsync(ctx))
            {
                await reply.WithEmbed(x => x.WithTitle("You are not a server administrator.").WithErrorColor())
                    .RespondAsync();
                return;
            }

            switch (subcommand.Name)
            {
                case "add":
                    if (ctx.GuildSettings.Extras.SelfRoleIds.Count is 25)
                        reply.WithEmbed(x => x.WithTitle("You can't have more than 25 self roles."));
                    else
                    {
                        var role = subcommand.GetOption("role").GetAsRole();
                        if (ctx.GuildSettings.Extras.SelfRoleIds.Contains(role.Id))
                            reply.WithEmbed(x => x.WithTitle("That role is already in the self role list."));
                        else
                        {
                            reply.WithEmbed(x => x.WithTitle("Added that role to the self role list."));
                            ctx.ModifyGuildSettings(data => data.Extras.SelfRoleIds.Add(role.Id));
                        }
                    }

                    break;
                case "remove":
                    reply.WithEmbedFrom("What roles would you like to remove from the self role list?")
                        .WithSelectMenu(
                            _getSelfRoleRemoveMenu(
                                ctx.GuildSettings.Extras.SelfRoleIds.Select(x => ctx.Guild.GetRole(x))));
                    break;
            }

            await reply.RespondAsync();
        }

        public override async Task HandleComponentAsync(MessageComponentContext ctx)
        {
            if (ctx.CustomIdParts[1] is "remove")
            {
                var selectedRoles = ctx.SelectedMenuOptions
                    .Select(ulong.Parse)
                    .Select(x => ctx.Guild.GetRole(x))
                    .ToArray();

                ctx.ModifyGuildSettings(data =>
                    data.Extras.SelfRoleIds.RemoveWhere(x => selectedRoles.Any(r => r.Id == x))
                );

                await ctx.CreateReplyBuilder()
                    .WithEphemeral()
                    .WithEmbed(x =>
                    {
                        x.WithTitle($"Removed the following {"role".ToQuantity(selectedRoles.Length)}");
                        x.WithDescription(selectedRoles.Select(r => r.Mention).Join(",\n"));
                    })
                    .RespondAsync();
            }
        }
    }
}