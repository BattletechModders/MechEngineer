using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
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
                if (cRef != null)
                {
                    if (__instance != MechLabPanel_InitWidgets_Patch.MechPropertiesWidget)
                    {
                        if (MechConfiguration.IsMechConfiguration(cRef?.Def))
                        {
                            cRef = null;
                            MechLabPanel_InitWidgets_Patch.MechPropertiesWidget.ShowHighlightFrame(true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
