using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Commands;

namespace Volte.Interactive
{
    public class Criteria<T> : ICriterion<T>
    {
        private List<ICriterion<T>> _critiera = new List<ICriterion<T>>();

        public Criteria<T> AddCriterion(ICriterion<T> criterion)
        {
            _critiera.Add(criterion);
            return this;
        }

        public Criteria<T> AddCriterionIf(bool condition, ICriterion<T> criterion)
        {
            if (condition)
                _critiera.Add(criterion);
            return this;
        }

        public async Task<bool> JudgeAsync(VolteContext sourceContext, T parameter)
        {
            foreach (var criterion in _critiera)
            {
                var result = await criterion.JudgeAsync(sourceContext, parameter);
                if (!result) return false;
            }
            return true;
        }
    }
}
