using Discord;
using Discord.WebSocket;
using Volte.UI.Avalonia.ViewModels;

namespace Volte.UI.Avalonia.Models;

public class GuildModel : BaseModel
{
    public GuildModel(SocketGuild guild)
    {
        Entity = guild;
        IconUrl = CDN.GetGuildIconUrl(Entity.Id, Entity.IconId, 128);
        SmallIconUrl = CDN.GetGuildIconUrl(Entity.Id, Entity.IconId, 64);
    }
    
    public SocketGuild Entity { get; init; }
    
    public string IconUrl { get; init; }
    
    public string SmallIconUrl { get; init; }
}