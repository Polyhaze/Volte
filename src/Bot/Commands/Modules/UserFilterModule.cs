using Starscript;
using Starscript.Internal;
using Volte.Interactive;
using ActionType = Volte.Entities.ActionType;

namespace Volte.Commands.Text.Modules;

[Group("UserFilter", "Uf")]
[RequireGuildAdmin]
public class UserFilterModule : VolteModule
{
    [Command, DummyCommand,
     Description("The set of commands used to create arbitrary filters for users joining & talking in your guild.")]
    public async Task<ActionResult> BaseAsync()
        => Ok(await TextCommandHelper.CreateCommandEmbedAsync(Context.Command, Context));

    [Command("Create", "Add", "New")]
    [Description(
        "Creates a user filter with the specified Starscript condition. This command is interactive and prompts for further data: what to do when the filter is triggered, and the reason.")]
    public Task<ActionResult> CreateAsync(
        [Remainder,
         Description(
             "The condition this filter should match. The result of the condition needs to be a boolean (true/false).")]
        string condition)
    {
        var rawCondition = condition.Replace("{", "").Replace("}", "");
        condition = $"{{{rawCondition}}}";

        if (!Parser.TryParse(condition, out var result))
            return BadRequest($"Syntax error: {result.Errors.First()}");

        var script = Compiler.SingleCompile(result);

        StringSegment conditionValue = null;
        try
        {
            conditionValue = VolteStarscript.Run(script, StarscriptHelper.Wrap(Context.User));

            _ = conditionValue.GetBooleanValue();
        }
        catch (StarscriptException se)
        {
            return BadRequest(se.Message);
        }
        catch (FormatException)
        {
            return BadRequest(
                $"Starscript result was not 'true' or 'false'. Evaluated result: {Format.Code(conditionValue!.ToString(), string.Empty)}");
        }

        return Ok(async () =>
        {
            GetActionType:
            await Context
                .CreateEmbed("What would you like to do when someone matches this filter? (Ban, Kick, Softban)")
                .SendToAsync(Context.Channel);
            var (actionType, didTimeout, _) = await Context.GetNextEnumAsync<ActionType>();
            if (didTimeout) return;
            if (!actionType.HasValue) goto GetActionType;

            GetReason:
            await Context.CreateEmbed("What would you like the reason to be for the moderation action?")
                .SendToAsync(Context.Channel);
            (var message, didTimeout) = await Context.GetNextAsync();
            if (didTimeout) return;
            if (!message.HasValue) goto GetReason;

            Context.Modify(data =>
                data.Extras.StarscriptTables.UserFilter.Add(rawCondition, actionType, message.Value.Content));

            await Context.CreateEmbed("Added that filter to this guild.").SendToAsync(Context.Channel);
        }, false);
    }

    [Command("Remove", "Delete", "Rem", "Del")]
    [Description("Deletes a user filter entry with the specified ID.")]
    public Task<ActionResult> RemoveAsync([Description("The filter entry ID.")] int id)
    {
        if (Context.GuildData.Extras.StarscriptTables.UserFilter.Entries.RemoveAll(x => x.Id == id) > 0)
        {
            Db.Save(Context.GuildData);
            return Ok("Removed that filter.");
        }

        return BadRequest(
            $"Unknown filter ID. See `{Context.FormatUsageFor("UserFilter List")}` to get the ID for a filter.");
    }

    [Command("List", "Ls")]
    [Description("Lists all user filters in this guild.")]
    public Task<ActionResult> ListAsync()
    {
        var entries = Context.GuildData.Extras.StarscriptTables.UserFilter.Entries;

        if (entries.None())
            return Ok("This guild has no user filters configured.");

        return Ok(new PaginatedMessage.Builder()
            .WithTitle("All configured user filters")
            .WithDefaults(Context)
            .WithPages(entries.Select(it
                => new StringBuilder($"ID: `{it.Id}`").AppendLine()
                    .AppendLine($"Script: {Format.Code(it.Starscript.CodeInput, string.Empty)}")
                    .AppendLine($"Action: `{it.Action}`")
                    .AppendLine($"Justification: `{it.Reason}`")
                    .ToString()
            ))
            .SplitPages(3));
    }

    [Command("Clear")]
    [Description("Clears all user filters in this guild.")]
    public Task<ActionResult> ClearAsync() =>
        Ok(async () =>
        {
            Confirmation:
            await Context.CreateEmbed("Are you sure you want to clear the user filters?")
                .SendToAsync(Context.Channel);
            var (confirmation, didTimeout, _) = await Context.GetNextAsync<bool>();
            if (didTimeout) return;
            if (!confirmation.HasValue) goto Confirmation;
            if (!confirmation.Value) return;

            Context.Modify(data => data.Extras.StarscriptTables.UserFilter.Entries.Clear());
            await Context.CreateEmbed("Done.").SendToAsync(Context.Channel);
        }, false);

    [Command("Check")]
    [Description(
        "Run the filter list against all users in this guild. WARNING: if your filters are misconfigured this can punish a LOT of people!")]
    public Task<ActionResult> CheckAsync() =>
        Ok(async () =>
        {
            var entriesList = Context.GuildData.Extras.StarscriptTables.UserFilter.Entries;

            if (entriesList.Count is 0)
            {
                await Context
                    .CreateEmbed(String(sb => sb
                            .AppendLine("There are no user filters configured in this guild.")
                            .Append($"To get started, run {Format.Code(Context.FormatUsageFor("UserFilter"))}")
                        )
                    ).SendToAsync(Context.Channel);
                return;
            }

            Confirmation:
            await Context.CreateEmbed("Are you sure you want to run the user filters on every user?")
                .SendToAsync(Context.Channel);
            var (confirmation, didTimeout, message) = await Context.GetNextAsync<bool>();
            if (didTimeout) return;
            if (!confirmation.HasValue) goto Confirmation;
            if (!confirmation.Value) return;

            await message.AddReactionAsync(Emojis.BallotBoxWithCheck);

            List<(UserFilterEntry Entry, Script Script)> compiledEntries = [];
            List<uint> brokenEntries = [];

            foreach (var entry in entriesList)
            {
                try
                {
                    compiledEntries.Add((entry, entry.Starscript.Compile()));
                }
                catch (ParseException)
                {
                    brokenEntries.Add(entry.Id);
                }
            }

            if (brokenEntries.Count > 0)
                Context.Modify(data => data.Extras.StarscriptTables.UserFilter.Entries
                    .RemoveAll(f => brokenEntries.Contains(f.Id)));

            if (compiledEntries.Count is 0)
            {
                await Context.CreateEmbed("All configured filters were broken and have been automatically removed.")
                    .SendToAsync(Context.Channel);
                return;
            }

            uint processedUsers = 0;
            uint actionedUsers = 0;

            var guild = await Context.Client.Rest.GetGuildAsync(Context.Guild.Id);

            IUserMessage resultMessage = null;

            await foreach (var users in guild.GetUsersAsync())
            {
                foreach (var u in users)
                {
                    processedUsers++;

                    if (await HandleUserAsync(u, compiledEntries))
                        actionedUsers++;
                }

                if (resultMessage is null)
                {
                    resultMessage = await Context.CreateEmbedBuilder()
                        .AddField("Processed users", processedUsers)
                        .AddField("Actioned users", actionedUsers)
                        .SendToAsync(Context.Channel);
                }
                else
                {
                    try
                    {
                        await resultMessage.ModifyAsync(x =>
                            x.Embed = Context.CreateEmbedBuilder()
                                .AddField("Processed users", processedUsers)
                                .AddField("Actioned users", actionedUsers)
                                .Build());
                    }
                    catch
                    {
                        resultMessage = await Context.CreateEmbedBuilder()
                            .AddField("Processed users", processedUsers)
                            .AddField("Actioned users", actionedUsers)
                            .SendToAsync(Context.Channel);
                    }
                }

                await Task.Delay(3.75.Seconds());
            }
        }, false);

    private async Task<bool> HandleUserAsync(
        IGuildUser user,
        List<(UserFilterEntry Entry, Script Script)> compiledEntries)
    {
        foreach (var (entry, script) in compiledEntries)
        {
            try
            {
                if (VolteStarscript.Hypervisor.Run(script, StarscriptHelper.Wrap(user)).GetBooleanValue())
                {
                    if (await entry.ExecuteAsync(Context.Guild, user))
                    {
                        Debug(LogSource.Service, $"{user} matched filter {entry.Id} in guild {Context.Guild.Id}");
                        return true;
                    }
                }
            }
            catch (FormatException)
            {
                // Ignore
            }
        }

        return false;
    }
}