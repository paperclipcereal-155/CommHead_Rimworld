using HarmonyLib;
using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace CommsHeadset
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_ApparelAdded))]
    public static class Patch_ApparelAdded
    {
        public static void Postfix(Pawn_ApparelTracker __instance)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.MapHeld == null) return;

            pawn.MapHeld
                .GetComponent<MapComponent_CommNetwork>()
                ?.Notify_ApparelChanged(pawn);
        }
    }

    [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_ApparelRemoved))]
    public static class Patch_ApparelRemoved
    {
        public static void Postfix(Pawn_ApparelTracker __instance)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.MapHeld == null) return;

            pawn.MapHeld
                .GetComponent<MapComponent_CommNetwork>()
                ?.Notify_ApparelChanged(pawn);
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.DeSpawn))]
    public static class Patch_PawnDeSpawn
    {
        public static void prefix(Pawn __instance)
        {
            if (__instance.MapHeld == null) return;

            __instance.MapHeld
                .GetComponent<MapComponent_CommNetwork>()
                ?.Notify_PawnDespawned(__instance);
        }
    }
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.CanInteractNowWith), new[] { typeof(Pawn), typeof(InteractionDef) })]
    public static class Patch_CanInteractWith
    {
        public static bool Prefix(Pawn_InteractionsTracker __instance, Pawn ___pawn, Pawn recipient, ref bool __result)
        {
            if (___pawn?.MapHeld == null || recipient?.MapHeld == null)
                return true;

            if (___pawn.MapHeld != recipient.MapHeld)
                return true;

            if (___pawn.Faction == null || ___pawn.Faction != recipient.Faction)
                return true;

            MapComponent_CommNetwork commHeadNetwork = ___pawn.MapHeld.GetComponent<MapComponent_CommNetwork>();
            if (commHeadNetwork == null)
                return true;

            if (commHeadNetwork.IsOnNetwork(___pawn) && commHeadNetwork.IsOnNetwork(recipient))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(GenSight), nameof(GenSight.LineOfSight), new[] { typeof(IntVec3), typeof(IntVec3), typeof(Map), typeof(bool), typeof(Func<IntVec3, bool>), typeof(int), typeof(int) })]
    public static class Patch_RadioLineOfSight
    {
        public static void Postfix(ref bool __result, IntVec3 start, IntVec3 end, Map map)
        {
            if (__result) return;

            Pawn pawn = start.GetFirstPawn(map);
            Pawn recipient = end.GetFirstPawn(map);

            if (pawn != null && recipient != null)
            {
                var commHeadNetwork = map.GetComponent<MapComponent_CommNetwork>();
                if (commHeadNetwork != null && commHeadNetwork.IsOnNetwork(pawn) && commHeadNetwork.IsOnNetwork(recipient))
                {
                    __result = true;
                }
            }
        }
    }

    [HarmonyPatch(typeof(ReachabilityImmediate), nameof(ReachabilityImmediate.CanReachImmediate), new[] { typeof(Pawn), typeof(LocalTargetInfo), typeof(PathEndMode) })]
    public static class Patch_RadioReachability
    {
        public static void Postfix(ref bool __result, Pawn pawn, LocalTargetInfo target, PathEndMode peMode)
        {
            if (__result) return;

            if (target.Thing is Pawn recipient && pawn.MapHeld != null)
            {
                var commHeadNetwork = pawn.MapHeld.GetComponent<MapComponent_CommNetwork>();
                if (commHeadNetwork != null && commHeadNetwork.IsOnNetwork(pawn) && commHeadNetwork.IsOnNetwork(recipient))
                {
                    __result = true;
                }
            }
        }
    }
}
