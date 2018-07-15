using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechBayPanel), "OnGotoMechLab")]
    internal static class MechBayPanel_OnGotoMechLab_Patch
    {
        public static void Prefix(MechBayPanel __instance, MechDef selectedMech)
        {
            try
            {
                if (!__instance.IsSimGame)
                {
                    return;
                }

                EnginePersistence.FixSimGameUID(__instance.Sim, selectedMech);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}