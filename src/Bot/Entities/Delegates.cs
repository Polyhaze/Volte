using Volte.Systems.Database.Entities;
using Volte.Systems.Database.EntitiesV2;

namespace Volte.Entities;

public delegate void DataEditor(GuildDataV2 data);
public delegate void TagInitializer(TagV2 tag);
public delegate void WarnInitializer(WarnV2 warn);
public delegate Task MessageCallback(IUserMessage message);
public delegate Task AsyncFunction();