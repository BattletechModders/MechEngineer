using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabPanel), "CreateMechComponentItem")]
    internal static class MechLabPanelCreateMechComponentItemPatch
    {
        public static void Prefix(MechLabPanel __instance, MechComponentRef componentRef)
        {
            try
            {
                if (!__instance.IsSimGame)
                {
                    return;
                }

                EnginePersistence.FixSimGameUID(__instance.sim, componentRef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}