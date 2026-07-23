using HarmonyLib;
using KMod;
using UnityEngine;

namespace OniSmartPriorities
{
    public sealed class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            SmartPrioritiesController.Configure(SmartPrioritiesConfig.Load(path));
            base.OnLoad(harmony);
            Debug.Log("[Smart Priorities] Mod loaded.");
        }
    }

    [HarmonyPatch(typeof(ScheduleManager), "OnSpawn")]
    internal static class ScheduleManagerOnSpawnPatch
    {
        private static void Postfix()
        {
            SmartPrioritiesController.Reset();
            SmartPrioritiesController.TryRebalance();
        }
    }

    [HarmonyPatch(typeof(MinionIdentity), nameof(MinionIdentity.Sim1000ms))]
    internal static class MinionIdentitySim1000msPatch
    {
        private static void Postfix()
        {
            SmartPrioritiesController.TryRebalance();
        }
    }
}
