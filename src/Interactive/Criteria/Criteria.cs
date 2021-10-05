using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Commands;

namespace Volte.Interactive
{
    public class Criteria<T> : ICriterion<T>
    {
        public delegate ValueTask<bool> LocalCriteria(SocketUserMessage ctx, T value);
        
        private readonly HashSet<ICriterion<T>> _critiera = new HashSet<ICriterion<T>>();
        private readonly HashSet<LocalCriteria> _localCriteria = new HashSet<LocalCriteria>();

        public Criteria<T> AddCriterion(ICriterion<T> criterion)
        {
            _critiera.Add(criterion);
            return this;
        }

        public Criteria<T> AddCriterion(LocalCriteria criteria)
        {
            _localCriteria.Add(criteria);
            return this;
        }

        public async ValueTask<bool> JudgeAsync(SocketUserMessage message, T parameter)
        {
            foreach (var criterion in _critiera)
            {
                var result = await criterion.JudgeAsync(message, parameter);
                if (!result) return false;
            }

            foreach (var criteria in _localCriteria)
            {
                var result = await criteria(message, parameter);
                if (!result) return false;
            }
            return true;
        }
    }
}
