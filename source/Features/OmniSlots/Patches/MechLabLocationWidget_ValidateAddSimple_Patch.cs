using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.OmniSlots.Patches
{
    [HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.ValidateAddSimple), typeof(MechComponentDef))]
    internal static class MechLabLocationWidget_ValidateAddSimple_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return OmniSlotsFeature.DisableHardpointValidatorsTranspiler(instructions);
        }

        internal static void Postfix(
            ref LocationLoadoutDef ___loadout,
            ref MechLabPanel ___mechLab,
            MechComponentDef newComponentDef,
            ref bool __result)
        {
            try
            {
                if (!__result)
                {
                    return;
                }

                var chassisDef = ___mechLab?.activeMechDef?.Chassis;

                if (chassisDef == null)
                {
                    return;
                }

                var location = ___loadout.Location;
                if (location == ChassisLocations.None)
                {

                    return;
                }

                __result = OmniSlotsFeature.Shared.ValidateAddSimple(chassisDef, location, newComponentDef);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}