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
            LongEventHandler.QueueLongEvent(() =>
            {
                var harmony = new Harmony("Hakurouken.CommHeadset");
                harmony.PatchAll();
            }, "InitializingCommsHeadset", false, null);
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

            listing.Label("Minimum Faction Tech Level to Spawn:");

            if (listing.ButtonText("Neolithic" + (Settings.minTechLevel == TechLevel.Neolithic ? " (ACTIVE)" : "")))
                Settings.minTechLevel = TechLevel.Neolithic;

            if (listing.ButtonText("Medieval" + (Settings.minTechLevel == TechLevel.Medieval ? " (ACTIVE)" : "")))
                Settings.minTechLevel = TechLevel.Medieval;

            if (listing.ButtonText("Industrial" + (Settings.minTechLevel == TechLevel.Industrial ? " (ACTIVE)" : "")))
                Settings.minTechLevel = TechLevel.Industrial;

            if (listing.ButtonText("Spacer" + (Settings.minTechLevel == TechLevel.Spacer ? " (ACTIVE)" : "")))
                Settings.minTechLevel = TechLevel.Spacer;

            if (listing.ButtonText("Ultra" + (Settings.minTechLevel == TechLevel.Ultra ? " (ACTIVE)" : "")))
                Settings.minTechLevel = TechLevel.Ultra;

            if (listing.ButtonText("Archotech" + (Settings.minTechLevel == TechLevel.Archotech ? " (ACTIVE)" : "")))
                Settings.minTechLevel = TechLevel.Archotech;

            listing.End();
        }

        public override string SettingsCategory() => "Comms Headset";
    }

}