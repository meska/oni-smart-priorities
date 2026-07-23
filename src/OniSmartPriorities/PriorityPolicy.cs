using System;
using System.Collections.Generic;
using System.Linq;

namespace OniSmartPriorities
{
    public static class PriorityPolicy
    {
        public static IReadOnlyDictionary<string, int> Assign(
            IReadOnlyDictionary<string, int> levels,
            SmartPrioritiesConfig config,
            int? fixedPriority = null)
        {
            if (levels.Count == 0)
            {
                return new Dictionary<string, int>();
            }

            if (fixedPriority.HasValue)
            {
                // Qua la skill no conta: tuti riceve la stessa priorità essenziale.
                return levels.Keys.ToDictionary(
                    workerId => workerId,
                    _ => fixedPriority.Value);
            }

            var distinctLevels = levels.Values
                .Distinct()
                .OrderByDescending(level => level)
                .ToArray();

            if (distinctLevels.Length == 1)
            {
                // Se xe tuti pari, nissun vien penalizà e il lavoro resta coperto.
                return levels.Keys.ToDictionary(
                    workerId => workerId,
                    _ => config.EqualLevelPriority);
            }

            var priorityByLevel = new Dictionary<int, int>();
            for (var rank = 0; rank < distinctLevels.Length; rank++)
            {
                var ratio = rank / (float)(distinctLevels.Length - 1);
                var scaled = config.MaximumPriority
                    - ratio * (config.MaximumPriority - config.MinimumPriority);
                priorityByLevel[distinctLevels[rank]] = (int)Math.Round(
                    scaled,
                    MidpointRounding.AwayFromZero);
            }

            return levels.ToDictionary(
                pair => pair.Key,
                pair => priorityByLevel[pair.Value]);
        }
    }
}
