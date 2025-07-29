namespace Volte.Systems.Commands.Text.Modules;

public sealed class HelpModule : VolteModule
{
    [Command("Help", "H")]
    [Description("Get help for Volte's many commands.")]
    public async Task<ActionResult> HelpAsync(
        [Remainder,
         Description(
             "The command or command group to search for. If you use pages or pager it will list every command in a paginator.")]
        string query = null)
    {
        if (query != null)
        {
            if (query.EqualsAnyIgnoreCase("pages", "pager"))
                return Ok(await GetPagesAsync().ToListAsync());

            var searchRes = CommandService.GetCommand(query);
            if (searchRes is null || !searchRes.IsAccessibleToGuild(Context.Guild.Id))
                return BadRequest($"No command or group found for {Format.Code(query)}.");

            return Ok(await TextCommandHelper.CreateCommandEmbedAsync(searchRes, Context));
        }

        var e = Context.CreateEmbedBuilder()
            .WithTitle("Command Help")
            .WithDescription(
                $"You can use {Format.Code(TextCommandHelper.FormatUsage(Context, CommandService.GetCommand("Help")))} for more details on a command or group.");

        var cmds = await GetAllRegularCommandsAsync().ToListAsync();
        var groupCmds = await GetAllGroupCommandsAsync().ToListAsync();

        try
        {
            if (cmds.Any()) e.AddField("Regular Commands", cmds.JoinToString(", "));
        }
        catch (ArgumentException)
        {
            e.AppendDescriptionLine().AppendDescriptionLine(cmds.JoinToString(", "));
        }

        try
        {
            if (groupCmds.Any()) e.AddField("Group Commands", groupCmds.JoinToString(", "));
        }
        catch (ArgumentException)
        {
            e.AppendDescriptionLine().AppendDescriptionLine(groupCmds.JoinToString(", "));
        }

        return Ok(e);
    }

    //module without aliases: regular module
    private async IAsyncEnumerable<string> GetAllRegularCommandsAsync()
    {
        foreach (var mdl in CommandService.GetAllModules().Where(x => !x.FullAliases.Any()))
        {
            if (!await TextCommandHelper.CanShowModuleAsync(Context, mdl)) continue;

            foreach (var cmd in mdl.Commands)
            {
                var fmt = TextCommandHelper.FormatCommandShort(cmd);
                if (fmt != null) yield return fmt;
            }
        }
    }

    // module with aliases: group command module
    private async IAsyncEnumerable<string> GetAllGroupCommandsAsync()
    {
        foreach (var mdl in CommandService.GetAllModules().Where(x => x.FullAliases.None()))
        {
            if (!await TextCommandHelper.CanShowModuleAsync(Context, mdl)) continue;

            var fmt = TextCommandHelper.FormatModuleShort(mdl);
            if (fmt != null) yield return fmt;
        }
    }

    private async IAsyncEnumerable<EmbedBuilder> GetPagesAsync() 
    {
        foreach (var cmd in await CommandService.GetAllCommands().WhereAccessibleAsync(Context).ToListAsync())
            yield return await TextCommandHelper.CreateCommandEmbedAsync(cmd, Context);
    }
}