using HarmonyLib;
using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace CommsHeadset
{
    public static class RadioSocialState
    {
        [ThreadStatic]
        public static bool IsSocialActive;
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
    public static class Patch_Pawn_SpawnSetup_RadioSync
    {
        public static void Postfix(Pawn __instance, Map map)
        {
            if (__instance.apparel == null || map == null) return;

            map.GetComponent<MapComponent_CommNetwork>()?.Notify_ApparelChanged(__instance);
        }
    }

    // --- APPAREL TRACKING ---
    [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_ApparelAdded))]
    public static class Patch_ApparelAdded
    {
        public static void Postfix(Pawn_ApparelTracker __instance)
        {
            __instance.pawn.MapHeld?.GetComponent<MapComponent_CommNetwork>()?.Notify_ApparelChanged(__instance.pawn);
        }
    }

    [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_ApparelRemoved))]
    public static class Patch_ApparelRemoved
    {
        public static void Postfix(Pawn_ApparelTracker __instance)
        {
            __instance.pawn.MapHeld?.GetComponent<MapComponent_CommNetwork>()?.Notify_ApparelChanged(__instance.pawn);
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.DeSpawn))]
    public static class Patch_PawnDeSpawn
    {
        public static void Prefix(Pawn __instance)
        {
            __instance.MapHeld?.GetComponent<MapComponent_CommNetwork>()?.Notify_PawnDespawned(__instance);
        }
    }

    [HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.CanInteractNowWith), new[] { typeof(Pawn), typeof(InteractionDef) })]
    public static class Patch_CanInteractWith_SameFactionOnly
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn ___pawn, Pawn recipient, ref bool __result)
        {

            if (___pawn?.MapHeld == null || recipient?.MapHeld == null) return true;
            if (___pawn.MapHeld != recipient.MapHeld) return true;


            if (___pawn.Faction == null || ___pawn.Faction != recipient.Faction)
                return true;

            var network = ___pawn.MapHeld.GetComponent<MapComponent_CommNetwork>();
            if (network != null && network.IsOnNetwork(___pawn) && network.IsOnNetwork(recipient))
            {
                RadioSocialState.IsSocialActive = true;
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix() => RadioSocialState.IsSocialActive = false;
    }

    [HarmonyPatch(typeof(GenSight), nameof(GenSight.LineOfSight))]
    [HarmonyPatch(new[] { typeof(IntVec3), typeof(IntVec3), typeof(Map), typeof(bool), typeof(Func<IntVec3, bool>), typeof(int), typeof(int) })]
    public static class Patch_RadioLineOfSight
    {
        public static void Postfix(ref bool __result)
        {
            if (RadioSocialState.IsSocialActive) __result = true;
        }
    }

    [HarmonyPatch(typeof(ReachabilityImmediate), nameof(ReachabilityImmediate.CanReachImmediate), new[] { typeof(Pawn), typeof(LocalTargetInfo), typeof(PathEndMode) })]
    public static class Patch_RadioReachability
    {
        public static void Postfix(ref bool __result, Pawn pawn, LocalTargetInfo target)
        {
            if (__result) return;

            if (target.Thing is Pawn recipient && pawn.MapHeld != null)
            {
                var network = pawn.MapHeld.GetComponent<MapComponent_CommNetwork>();
                if (network != null && network.IsOnNetwork(pawn) && network.IsOnNetwork(recipient))
                {
                    __result = true;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PawnApparelGenerator), nameof(PawnApparelGenerator.GenerateStartingApparelFor))]
    public static class Patch_ApparelGen_HeadsetInjection
    {
        public static void Postfix(Pawn pawn)
        {

            if (pawn?.Faction?.def == null) return;


            if (pawn.Faction.IsPlayer) return;

            if (pawn.Faction.def.techLevel < CommHeadsetMod.Settings.minTechLevel) return;

            if (Rand.Value <= CommHeadsetMod.Settings.spawnRate)
            {
                ThingDef headsetDef = DefDatabase<ThingDef>.GetNamed("Apparel_CommHeadset", false);
                if (headsetDef != null)
                {
                    pawn.apparel.Wear((Apparel)ThingMaker.MakeThing(headsetDef));
                }
            }
        }
    }
}