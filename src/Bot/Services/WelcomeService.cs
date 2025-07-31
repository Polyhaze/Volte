using LiteDB;
using Volte.Systems.Database;
using Volte.Systems.Database.Entities;
using Volte.Systems.Database.EntitiesV2;

namespace Volte.Services;

public sealed class WelcomeService : VolteService
{
    private readonly DatabaseService _db;
    
    public WelcomeService(DatabaseService databaseService)
    {
        _db = databaseService;
    }
    
    public async Task JoinAsync(UserJoinedEventArgs args)
    {
        if (!Config.EnabledFeatures.Welcome) return;
        
        var data = _db.GetData(args.Guild);

        await DmAsync(args, data);

        if (data.Settings.Welcome.JoinMessage.IsNullOrEmpty())
            return; //we don't want to send an empty join message


        Debug(LogSource.Volte,
            "User joined a guild, let's check to see if we should send a welcome embed.");
        var welcomeMessage = await data.Settings.Welcome.FormatJoinMessageAsync(args.User);
        var c = args.Guild.GetTextChannel(data.Settings.Welcome.Channel);

        if (c is not null)
        {
            await new EmbedBuilder()
                .WithColor(data.Settings.Welcome.EmbedColor)
                .WithDescription(welcomeMessage)
                .WithThumbnailUrl(args.User.GetEffectiveAvatarUrl())
                .WithCurrentTimestamp()
                .SendToAsync(c);

            Debug(LogSource.Volte, $"Sent a welcome embed to #{c.Name}.");
        } else
            Debug(LogSource.Volte,
            "WelcomeChannel config value resulted in an invalid/nonexistent channel; aborting.");
    }
    
    public async Task DmAsync(UserJoinedEventArgs args, GuildDataV2 data = null)
    {
        if (!Config.EnabledFeatures.Welcome) return;
        
        data ??= _db.GetData(args.Guild);
        
        if (!data.Settings.Welcome.JoinDmMessage.IsNullOrEmpty())
            await args.User.TrySendMessageAsync(await data.Settings.Welcome.FormatJoinDmMessageAsync(args.User));
    }

    public async Task LeaveAsync(UserLeftEventArgs args)
    {
        if (!Config.EnabledFeatures.Welcome) return;
        
        var data = _db.GetData(args.Guild);
        if (data.Settings.Welcome.LeftMessage.IsNullOrEmpty()) return;
        Debug(LogSource.Volte,
            "User left a guild, let's check to see if we should send a leaving embed.");
        var c = args.Guild.GetTextChannel(data.Settings.Welcome.Channel);
        if (c is not null)
        {
            await new EmbedBuilder()
                .WithColor(data.Settings.Welcome.EmbedColor)
                .WithDescription(await data.Settings.Welcome.FormatLeftMessageAsync(args.Guild, args.User))
                .WithThumbnailUrl(args.User.GetDisplayAvatarUrl())
                .WithCurrentTimestamp()
                .SendToAsync(c);
            Debug(LogSource.Volte, $"Sent a leaving embed to #{c.Name}.");
        } else
            Debug(LogSource.Volte,
                "WelcomeChannel config value resulted in an invalid/nonexistent channel; aborting.");
    }
}