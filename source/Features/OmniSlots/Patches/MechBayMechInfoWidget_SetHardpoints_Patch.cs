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

                static void SetData(TextMeshProUGUI text, HardpointStat stat)
                {
                    text.gameObject.SetActive(stat.ShowStat);
                    text.SetText(stat.HardpointString);
                }

                SetData(___ballisticHardpointText, calc.Ballistic);
                SetData(___energyHardpointText, calc.Energy);
                SetData(___missileHardpointText, calc.Missile);
                SetData(___smallHardpointText, calc.Small);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}