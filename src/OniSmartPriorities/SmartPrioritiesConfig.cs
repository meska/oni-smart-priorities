using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace OniSmartPriorities
{
    public sealed class SmartPrioritiesConfig
    {
        public int MinimumPriority { get; set; } = 2;

        public int EqualLevelPriority { get; set; } = 3;

        public int MaximumPriority { get; set; } = 5;

        public float RebalanceIntervalSeconds { get; set; } = 60f;

        public static SmartPrioritiesConfig Load(string modPath)
        {
            var configPath = Path.Combine(modPath, "config.json");

            try
            {
                if (!File.Exists(configPath))
                {
                    Debug.LogWarning("[Smart Priorities] config.json not found; using defaults.");
                    return new SmartPrioritiesConfig();
                }

                var config = JsonConvert.DeserializeObject<SmartPrioritiesConfig>(
                    File.ReadAllText(configPath));

                if (config == null || !config.IsValid())
                {
                    Debug.LogWarning("[Smart Priorities] Invalid config.json; using defaults.");
                    return new SmartPrioritiesConfig();
                }

                return config;
            }
            catch (Exception exception)
            {
                // JSON storto? Meio i default che blocar tuti i lavori.
                Debug.LogWarning(
                    $"[Smart Priorities] Cannot read config.json; using defaults. {exception.Message}");
                return new SmartPrioritiesConfig();
            }
        }

        public bool IsValid()
        {
            return MinimumPriority >= 1
                && MinimumPriority < EqualLevelPriority
                && EqualLevelPriority < MaximumPriority
                && MaximumPriority <= 5
                && RebalanceIntervalSeconds >= 5f
                && RebalanceIntervalSeconds <= 600f;
        }
    }
}
