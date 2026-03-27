using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;

namespace CommsHeadset
{
    public class CommsHeadsetSettings : ModSettings
    {
        public bool allowAlliesToSpawn = false;
        public bool allowEnemiesToSpawn = false;

        public float spawnRate = 0.2f;
        public TechLevel minTechLevel = TechLevel.Industrial;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref allowAlliesToSpawn, "allowAlliesToSpawn", false);
            Scribe_Values.Look(ref allowEnemiesToSpawn, "allowEnemiesToSpawn", false);
            Scribe_Values.Look(ref spawnRate, "spawnRate", 0.2f);
            Scribe_Values.Look(ref minTechLevel, "minTechLevel", TechLevel.Industrial);
        }
    }

    public class CommHeadsetMod : Mod
    {
        public static CommsHeadsetSettings Settings;

        public CommHeadsetMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<CommsHeadsetSettings>();
            new Harmony("Hakurouken.CommHeadset").PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.CheckboxLabeled("Allow Allies to spawn with Headsets", ref Settings.allowAlliesToSpawn);
            listing.CheckboxLabeled("Allow Enemies to spawn with Headsets", ref Settings.allowEnemiesToSpawn);

            listing.Gap();

            listing.Label($"Spawn Chance: {Settings.spawnRate.ToStringPercent()}");
            Settings.spawnRate = listing.Slider(Settings.spawnRate, 0f, 1f);

            listing.Gap();


            listing.Label($"Minimum Tech Level: {Settings.minTechLevel.ToString()}");

            float techVal = (float)Settings.minTechLevel;
            techVal = listing.Slider(techVal, 1f, 7f);
            Settings.minTechLevel = (TechLevel)Mathf.RoundToInt(techVal);

            listing.End();
        }

        public override string SettingsCategory() => "Comms Headset";
    }

}