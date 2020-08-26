using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.OmniSlots.Patches
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
        public static void Postfix(
            MechLabLocationWidget __instance,
            MechComponentRef cRef,
            WeaponDef wDef,
            bool isOriginalLocation,
            bool canBeAdded, 
            ref LocationDef ___chassisLocationDef)
        {
            try
            {
                if (cRef == null || wDef == null || ___chassisLocationDef.Hardpoints == null)
                {
                    return;
                }

                var hasOmni = ___chassisLocationDef.Hardpoints.Any(x => x.Omni);
                if (hasOmni)
                {
                    __instance.ShowHighlightFrame(true, (!isOriginalLocation) ? ((!canBeAdded) ? UIColor.GoldHalf : UIColor.Gold) : UIColor.Blue);
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}