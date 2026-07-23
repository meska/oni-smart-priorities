using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OniSmartPriorities
{
    public static class SmartPrioritiesController
    {
        private static SmartPrioritiesConfig config = new SmartPrioritiesConfig();
        private static float nextRebalanceTime = -1f;

        public static void Configure(SmartPrioritiesConfig loadedConfig)
        {
            config = loadedConfig ?? new SmartPrioritiesConfig();
        }

        public static void Reset()
        {
            // Un nuovo save riparte da zero, no dal tempo della colonia precedente.
            nextRebalanceTime = -1f;
        }

        public static void TryRebalance()
        {
            if (GameClock.Instance == null || Db.Get() == null)
            {
                return;
            }

            var now = GameClock.Instance.GetTime();
            if (nextRebalanceTime >= 0f && now < nextRebalanceTime)
            {
                return;
            }

            nextRebalanceTime = now + config.RebalanceIntervalSeconds;

            try
            {
                Rebalance();
            }
            catch (Exception exception)
            {
                // La mod no ga mai da fermar el sim se Klei cambia una API.
                Debug.LogError(
                    $"[ONI Smart Priorities] Rebalance failed: {exception}");
            }
        }

        private static void Rebalance()
        {
            var workers = Components.LiveMinionIdentities.Items
                .Where(identity => identity != null && !identity.HasTag(GameTags.Dead))
                .Select(identity => new Worker(identity))
                .Where(worker => worker.Consumer != null)
                .ToList();

            if (workers.Count == 0)
            {
                return;
            }

            var changedPriorities = 0;
            var uncoveredGroups = new List<string>();

            foreach (var group in Db.Get().ChoreGroups.resources)
            {
                if (!group.userPrioritizable)
                {
                    continue;
                }

                var eligibleWorkers = workers
                    .Where(worker => !worker.Consumer.IsChoreGroupDisabled(group))
                    .ToList();

                if (eligibleWorkers.Count == 0)
                {
                    uncoveredGroups.Add(group.Name);
                    continue;
                }

                var levels = eligibleWorkers.ToDictionary(
                    worker => worker.Id,
                    worker => worker.Consumer.GetAssociatedSkillLevel(group));
                var priorities = PriorityPolicy.Assign(levels, config);

                foreach (var worker in eligibleWorkers)
                {
                    var targetPriority = priorities[worker.Id];
                    if (worker.Consumer.GetPersonalPriority(group) == targetPriority
                        && worker.Consumer.IsPermittedByUser(group))
                    {
                        continue;
                    }

                    worker.Consumer.SetPersonalPriority(group, targetPriority);
                    changedPriorities++;
                }
            }

            if (uncoveredGroups.Count > 0)
            {
                Debug.LogWarning(
                    "[ONI Smart Priorities] No eligible duplicant for: "
                    + string.Join(", ", uncoveredGroups));
            }

            if (changedPriorities > 0)
            {
                Debug.Log(
                    $"[ONI Smart Priorities] Updated {changedPriorities} priorities "
                    + $"across {workers.Count} duplicants.");
            }
        }

        private sealed class Worker
        {
            public Worker(MinionIdentity identity)
            {
                Consumer = identity.GetComponent<ChoreConsumer>();
                Id = identity.GetComponent<KPrefabID>().InstanceID.ToString();
            }

            public string Id { get; }

            public ChoreConsumer Consumer { get; }
        }
    }
}
