using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using SVGImporter;

namespace MechEngineer.Features.DynamicSlots.Patches
{
    // support no-icon MechComponents
    [HarmonyPatch(typeof(MechLabItemSlotElement), "SetIconAndText")]
    public static class MechLabItemSlotElement_SetIconAndText_Patch
    {
        public static void Postfix(MechComponentRef ___componentRef, SVGImage ___icon)
        {
            try
            {
                ___icon.gameObject.SetActive(!string.IsNullOrEmpty(___componentRef.Def.Description.Icon));
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}