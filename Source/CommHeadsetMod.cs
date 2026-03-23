using HarmonyLib;
using Verse;

namespace CommsHeadset
{
    public class CommHeadsetMod : Mod
    {
        public CommHeadsetMod(ModContentPack content) : base (content) 
        {
            new Harmony("Hakurouken.CommHeadset").PatchAll ();
        }
    }
}
