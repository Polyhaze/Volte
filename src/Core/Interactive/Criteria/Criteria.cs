using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Commands;

namespace Volte.Interactive
{
    public class Criteria<T> : ICriterion<T>
    {
        private List<ICriterion<T>> _critiera = new List<ICriterion<T>>();
        private List<Func<VolteContext, T, Task<bool>>> _localCriteria = new List<Func<VolteContext, T, Task<bool>>>();

        public Criteria<T> AddCriterion(ICriterion<T> criterion)
        {
            _critiera.Add(criterion);
            return this;
        }

        public Criteria<T> AddCriterion(Func<VolteContext, T, Task<bool>> criteria)
        {
            _localCriteria.Add(criteria);
            return this;
        }

        public async Task<bool> JudgeAsync(VolteContext sourceContext, T parameter)
        {
            foreach (var criterion in _critiera)
            {
                var result = await criterion.JudgeAsync(sourceContext, parameter);
                if (!result) return false;
            }

            foreach (var criteria in _localCriteria)
            {
                var result = await criteria(sourceContext, parameter);
                if (!result) return false;
            }
            return true;
        }
    }
}
