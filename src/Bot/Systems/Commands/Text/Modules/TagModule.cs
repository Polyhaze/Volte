using Volte.Systems.Database.EntitiesV2;

namespace Volte.Systems.Commands.Text.Modules;

[Group("Tags")]
public class TagModule : VolteModule
{
    [Command, DummyCommand, Description("The command group for modifying and creating tags.")]
    public async Task<ActionResult> BaseAsync() =>
        Ok(await TextCommandHelper.CreateCommandEmbedAsync(Context.Command, Context));

    [Command("Stats")]
    [Description("Shows stats for a tag.")]
    public async Task<ActionResult> TagStatsAsync([Remainder, Description("The tag to show stats for.")]
        TagV2 tag)
    {
        var u = await Context.Client.Rest.GetUserAsync(tag.CreatorId);
        
        return Ok(Context.CreateEmbedBuilder()
            .WithTitle($"Tag {tag.Name}")
            .AddField("Aliases", tag.Aliases.FormatCollection(static x => Format.Code(x), separator: ", "), true)
            .AddField("Creator", $"{u}", true)
            .AddField("Uses", $"**{tag.Uses}**", true)
            .AddField("Response", Format.Code(tag.Response, string.Empty))
        );
    }

    [Command("List", "Ls")]
    [Description("Lists all available tags in the current guild.")]
    public Task<ActionResult> TagsAsync()
        => Ok(Context.CreateEmbedBuilder(
            Context.GuildData.Settings.Collections.Tags.FormatCollection(static x => Format.Code(x.Name), separator: ", ")
        ).WithTitle($"Available Tags for {Context.Guild.Name}"));

    [Command("Create", "Add", "New")]
    [Description("Creates a tag with the specified name and response (in that order).")]
    [RequireGuildModerator]
    public async Task<ActionResult> TagCreateAsync(
        [Description(
            "The name of the tag you want to make. If you want spaces in the name, make sure to surround it in \".")]
        string name, [Remainder, Description("What you want the tag to reply with.")]
        string response)
    {
        if (Context.GuildData.GetTagByNameOrAlias(name).TryGet(out var tag))
        {
            var user = await Context.Client.Rest.GetUserAsync(tag.CreatorId);

            var desiredNameIsAliasOfFound = tag.Aliases.ContainsIgnoreCase(name);

            return BadRequest(desiredNameIsAliasOfFound 
                ? $"Cannot make the tag `{name}`, as it already exists as an alias of the tag **{tag.Name}** and is owned by **{user}**." 
                : $"Cannot make the tag `{name}`, as it already exists and is owned by **{user}**.");
        }

        tag = new()
        {
            Name = name,
            Response = response,
            CreatorId = Context.User.Id,
            GuildId = Context.Guild.Id,
            Uses = default
        };

        Context.Modify(data => data.AddTag(tag));

        return Ok(Context.CreateEmbedBuilder()
            .WithTitle("Tag Created!")
            .AddField("Name", tag.Name, true)
            .AddField("Creator", Context.User.Mention, true)
            .AddField("Response",
                tag.Response.Length > EmbedFieldBuilder.MaxFieldValueLength
                    ? "<Cannot display; too large.>"
                    : tag.Response));
    }

    [Command("Edit", "Ed", "E")]
    [Description("Edit a tag's content if it exists.")]
    [RequireGuildModerator]
    public Task<ActionResult> TagEditAsync([Description("The tag whose content you want to modify.")]
        TagV2 tag, [Remainder, Description("The new content of the tag.")]
        string response)
    {
        Context.GuildData.Settings.Collections.Tags.Remove(tag);
        tag.Response = response;
        Context.GuildData.Settings.Collections.Tags.Add(tag);
        Db.Save(Context.GuildData);
        return Ok($"Successfully modified the content of tag **{tag.Name}**.");
    }
    
    [Command("AddAlias", "+A")]
    [Description("Add an alias to an existing tag.")]
    [RequireGuildModerator]
    public Task<ActionResult> AddAliasAsync([Description("The tag whose aliases you want to modify.")]
        TagV2 tag, 
        [Remainder, Description("The new alias of the tag.")]
        string alias)
    {
        Context.GuildData.Settings.Collections.Tags.Remove(tag);
        tag.Aliases.Add(alias);
        Context.GuildData.Settings.Collections.Tags.Add(tag);
        Db.Save(Context.GuildData);
        return Ok($"Successfully added the alias `{alias}` to the tag **{tag.Name}**.");
    }
    
    [Command("RemoveAlias", "RemAlias", "-A")]
    [Description("Remove an alias from an existing tag.")]
    [RequireGuildModerator]
    public Task<ActionResult> RemoveAliasAsync([Description("The tag whose aliases you want to modify.")]
        TagV2 tag, 
        [Remainder, Description("The alias of the tag to remove.")]
        string alias)
    {
        Context.GuildData.Settings.Collections.Tags.Remove(tag);
        var removed = tag.Aliases.Remove(alias);
        Context.GuildData.Settings.Collections.Tags.Add(tag);
        Db.Save(Context.GuildData);
        
        return Ok(removed 
            ? $"Successfully removed the alias `{alias}` to the tag **{tag.Name}**." 
            : $"Alias `{alias}` not found on tag **{tag.Name}**.");
    }

    [Command("Delete", "Remove", "Del", "Rem")]
    [Description("Deletes a tag if it exists.")]
    [RequireGuildModerator]
    public async Task<ActionResult> TagDeleteAsync([Remainder, Description("The tag to delete.")]
        TagV2 tag)
    {
        Context.GuildData.Settings.Collections.Tags.Remove(tag);
        Db.Save(Context.GuildData);
        return Ok($"Deleted the tag {Format.Bold(tag.Name)}, created by " +
                  $"**{await Context.Client.Rest.GetUserAsync(tag.CreatorId)}**, with " +
                  $"{Format.Bold("use".ToQuantity(tag.Uses))}.");
    }
}