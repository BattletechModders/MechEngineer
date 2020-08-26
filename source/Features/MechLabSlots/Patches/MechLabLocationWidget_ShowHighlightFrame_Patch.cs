using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.Patches
{
    [HarmonyPatch(
        typeof(MechLabLocationWidget),
        nameof(MechLabLocationWidget.ShowHighlightFrame),
        typeof(MechComponentRef),
        typeof(WeaponDef),
        typeof(bool),
        typeof(bool)
        )]
    public static class MechLabLocationWidget_ShowHighlightFrame_Patch
    {
        public static void Prefix(MechLabLocationWidget __instance, ref MechComponentRef cRef)
        {
            try
            {
                MechPropertiesWidget.ShowHighlightFrame(__instance, ref cRef);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}
