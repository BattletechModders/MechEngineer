using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.OmniSlots.Patches
{
    [HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.RefreshHardpointData))]
    internal static class MechLabLocationWidget_RefreshHardpointData_Patch
    {
        internal static void Postfix(
            MechLabLocationWidget __instance,
            ref LocationDef ___chassisLocationDef,

            ref int ___currentBallisticCount,
            ref int ___currentEnergyCount,
            ref int ___currentMissileCount,
            ref int ___currentSmallCount,

            ref int ___totalBallisticHardpoints,
            ref int ___totalEnergyHardpoints,
            ref int ___totalMissileHardpoints,
            ref int ___totalSmallHardpoints,

            ref MechLabHardpointElement[] ___hardpoints,
            ref List<MechLabItemSlotElement> ___localInventory)
        {
            try
            {
                var inventory = ___localInventory.Select(x => x.ComponentRef.Def);
                var hardpoints = ___chassisLocationDef.Hardpoints;
                if (hardpoints == null)
                {
                    // how can this happen? is this from the properties widget?
                    Control.Logger.Debug?.Log($"hardpoints is null in location={__instance.loadout?.Location}");
                    return;
                }
                
                var calc = new HardpointOmniUsageCalculator(inventory, hardpoints);
                
                Control.Logger.Debug?.Log(calc);

                ___currentBallisticCount = calc.Ballistic.VanillaUsage;
                ___currentEnergyCount = calc.Energy.VanillaUsage;
                ___currentMissileCount = calc.Missile.VanillaUsage;
                ___currentSmallCount = calc.Small.VanillaUsage;

                ___totalBallisticHardpoints = calc.Ballistic.DynamicMax;
                ___totalEnergyHardpoints = calc.Energy.DynamicMax;
                ___totalMissileHardpoints = calc.Missile.DynamicMax;
                ___totalSmallHardpoints = calc.Small.DynamicMax;

                static void SetData(MechLabHardpointElement element, HardpointStat stat)
                {
                    //element.gameObject.SetActive(stat.ShowStat);
                    element.SetData(stat.CategoryForLocationWidget, stat.HardpointString);
                }

                SetData(___hardpoints[0], calc.Ballistic);
                SetData(___hardpoints[1], calc.Energy);
                SetData(___hardpoints[2], calc.Missile);
                SetData(___hardpoints[3], calc.Small);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}