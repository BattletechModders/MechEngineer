using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechBayPanel), "OnRepairMech")]
    internal static class MechBayPanel_OnRepairMech_Patch
    {
        public static void Prefix(MechBayPanel __instance, MechBayMechUnitElement mechElement)
        {
            try
            {
                if (!__instance.IsSimGame)
                {
                    return;
                }

                if (mechElement == null || mechElement.MechDef == null)
                {
                    return;
                }

                EnginePersistence.FixSimGameUID(__instance.Sim, mechElement.MechDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}