using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Commands;

namespace Volte.Interactive
{
    public class EnsureFromUserCriterion : ICriterion<IMessage>
    {
        private readonly ulong _id;

        public EnsureFromUserCriterion(IUser user)
            => _id = user.Id;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public EnsureFromUserCriterion(ulong id)
            => _id = id;

        public ValueTask<bool> JudgeAsync(SocketUserMessage message, IMessage parameter) 
            => new ValueTask<bool>(_id == parameter.Author.Id);

    }
}
