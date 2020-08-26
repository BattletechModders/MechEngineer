using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using MechEngineer.Features.DynamicSlots;
using TMPro;

namespace MechEngineer.Features.OmniSlots.Patches
{
    [HarmonyPatch(typeof(MechBayMechInfoWidget), "SetHardpoints")]
    internal static class MechBayMechInfoWidget_SetHardpoints_Patch
    {
        internal static void Postfix(
            MechDef ___selectedMech,
            TextMeshProUGUI ___ballisticHardpointText,
            TextMeshProUGUI ___energyHardpointText,
            TextMeshProUGUI ___missileHardpointText,
            TextMeshProUGUI ___smallHardpointText)
        {
            try
            {
                if (___selectedMech == null)
                {
                    return;
                }
                
                var inventory = ___selectedMech.Inventory.Select(x => x.Def);
                var hardpoints = MechDefBuilder.Locations.SelectMany(x => ___selectedMech.Chassis.GetLocationDef(x).Hardpoints).ToArray();

                var calc = new HardpointOmniUsageCalculator(inventory, hardpoints);

                Control.Logger.Debug?.Log(calc);

                ___ballisticHardpointText.SetText(calc.Ballistic.HardpointString);
                ___energyHardpointText.SetText(calc.Energy.HardpointString);
                ___missileHardpointText.SetText(calc.Missile.HardpointString);
                ___smallHardpointText.SetText(calc.Small.HardpointString);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}
