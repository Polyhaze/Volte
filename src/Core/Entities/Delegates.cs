using System.Threading.Tasks;
using Discord;

namespace Volte.Core.Entities
{
    public delegate void DataEditor(GuildData data);
    public delegate void TagInitializer(Tag tag);
    public delegate void WarnInitializer(Warn warn);
    public delegate Task MessageCallback(IUserMessage message);
    public delegate Task AsyncFunction();
}