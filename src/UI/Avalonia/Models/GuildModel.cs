using Discord;
using Discord.WebSocket;
using Volte.Systems.Database.EntitiesV2;
using Volte.UI.Avalonia.ViewModels;

namespace Volte.UI.Avalonia.Models;

public class GuildModel : BaseModel
{
    public GuildModel(SocketGuild guild, GuildDataV2 guildData)
    {
        Entity = guild;
        Data = guildData;
        IconUrl = CDN.GetGuildIconUrl(Entity.Id, Entity.IconId, 128);
        SmallIconUrl = CDN.GetGuildIconUrl(Entity.Id, Entity.IconId, 64);
    }
    
    public SocketGuild Entity { get; init; }
    
    public GuildDataV2 Data { get; init; }
    
    public string IconUrl { get; init; }
    
    public string SmallIconUrl { get; init; }

    public static IEnumerable<GuildModel> MapDataToGuilds(HashSet<GuildDataV2> guildData, IReadOnlyCollection<SocketGuild> guilds) 
        => guilds.Select(g => 
            new GuildModel(g, guildData.First(d => d.Id == g.Id))
        );
}