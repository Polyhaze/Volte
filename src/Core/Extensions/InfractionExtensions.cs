using System;
using System.Collections.Generic;
using System.Text;

namespace BrackeysBot
{
    public static class InfractionExtensions
    {
        public static InfractionType AsInfractionType(this TemporaryInfractionType t)
        {
            switch(t)
            {
                case TemporaryInfractionType.TempBan: return InfractionType.TemporaryBan;
                case TemporaryInfractionType.TempMute: return InfractionType.TemporaryMute;
                default: throw new ArgumentException($"{nameof(InfractionType)} does not contain a valid counterpart for {t}.");
            }
        }
    }
}
