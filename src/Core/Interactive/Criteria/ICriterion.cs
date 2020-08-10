using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Commands;

namespace Volte.Interactive
{
    public interface ICriterion<in T>
    {
        Task<bool> JudgeAsync(VolteContext sourceContext, T parameter);
    }
}
