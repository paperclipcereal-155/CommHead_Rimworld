using HarmonyLib;
using UnityEngine;
using Verse;

namespace CommsHeadset
{
    public class CommsHeadsetSettings : ModSettings
    {
        public bool includeAllies = false;
        public bool includeEnemies = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref includeAllies, "includeAllies", false);
            Scribe_Values.Look(ref includeEnemies, "includeEnemies", false);
        }
    }

    public class CommHeadsetMod : Mod
    {
        public static CommsHeadsetSettings Settings;

        public CommHeadsetMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<CommsHeadsetSettings>();

            var harmony = new Harmony("Hakurouken.CommHeadset");
            harmony.PatchAll();

            Log.Message("[CommsHeadset] Mod Loaded. Settings initialized.");
        }

    }
}