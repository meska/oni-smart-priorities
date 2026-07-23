using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using UnityEngine;

namespace OniSmartPriorities
{
    public sealed class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            SmartPrioritiesController.Configure(SmartPrioritiesConfig.Load(path));
            Debug.Log("[Smart Priorities] Mod loaded.");
        }
    }

    [HarmonyPatch(typeof(MinionConfig), nameof(MinionConfig.CreatePrefab))]
    internal static class MinionConfigCreatePrefabPatch
    {
        private static void Postfix(GameObject __result)
        {
            __result.AddOrGet<SmartPrioritiesState>();
        }
    }

    [HarmonyPatch(typeof(BionicMinionConfig), nameof(BionicMinionConfig.CreatePrefab))]
    internal static class BionicMinionConfigCreatePrefabPatch
    {
        private static void Postfix(GameObject __result)
        {
            __result.AddOrGet<SmartPrioritiesState>();
        }
    }

    [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
    internal static class DetailsScreenOnPrefabInitPatch
    {
        private static void Postfix()
        {
            PUIUtils.AddSideScreenContent<SmartPrioritiesSideScreen>();
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
