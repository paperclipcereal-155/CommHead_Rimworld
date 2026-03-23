using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CommsHeadset
{
    [StaticConstructorOnStartup]
    public class MapComponent_CommNetwork : MapComponent
    {
        private readonly
            HashSet<Pawn> _wearers = new HashSet<Pawn>();

        public MapComponent_CommNetwork(Map map) : base(map) { }

        public void Notify_ApparelChanged(Pawn pawn)
        {
            if (PawnWearingHeadset(pawn))
                _wearers.Add(pawn);
            else
                _wearers.Remove(pawn);
        }

        public void Notify_PawnDespawned(Pawn pawn)
        {
            _wearers.Remove(pawn);
        }

        public bool IsOnNetwork(Pawn pawn)
        {
            return _wearers.Contains(pawn);
        }

        private static bool PawnWearingHeadset(Pawn pawn)
        {
            if (pawn.apparel == null)
                return false;
            foreach (Apparel apparel in pawn.apparel.WornApparel)
            {
                if (apparel.def.apparel.tags != null &&
                    apparel.def.apparel.tags.Contains("CommHeadset"))
                    return true;
            }
            return false;
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            RebuildCache();

            Log.Message($"[CommsHeadset] Network initialized for Map {map.uniqueID}. Indexing pawns...");
        }

        public void RebuildCache()
        {

            foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
            {

                Notify_ApparelChanged(pawn);
            }
        }
    }
}
