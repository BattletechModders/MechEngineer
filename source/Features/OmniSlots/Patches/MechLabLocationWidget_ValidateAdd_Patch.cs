using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.OmniSlots.Patches
{
    [HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.ValidateAdd), typeof(MechComponentDef))]
    internal static class MechLabLocationWidget_ValidateAdd_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return OmniSlotsFeature.Transpiler(instructions);
        }

        internal static void Postfix(
            ref LocationDef ___chassisLocationDef,

            MechComponentDef newComponentDef,

            ref List<MechLabItemSlotElement> ___localInventory,
            ref bool __result)
        {
            try
            {
                if (!__result)
                {
                    return;
                }

                __result = OmniSlotsFeature.Shared.ValidateAdd(ref ___localInventory, ref ___chassisLocationDef, newComponentDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}