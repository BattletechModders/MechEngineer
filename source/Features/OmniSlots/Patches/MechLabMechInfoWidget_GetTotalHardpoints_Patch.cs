using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using MechEngineer.Features.DynamicSlots;

namespace MechEngineer.Features.OmniSlots.Patches
{
    [HarmonyPatch(typeof(MechLabMechInfoWidget), "GetTotalHardpoints")]
    public static class MechLabMechInfoWidget_GetTotalHardpoints_Patch
    {
        // only allow one engine part per specific location
        public static void Postfix(
            MechLabMechInfoWidget __instance,
            MechLabPanel ___mechLab,
            MechLabHardpointElement[] ___hardpoints
        )
        {
            try
            {
                var mechDef = ___mechLab.CreateMechDef();
                if (mechDef == null)
                {
                    return;
                }

                var inventory = mechDef.Inventory.Select(x => x.Def);
                var hardpoints = MechDefBuilder.Locations.SelectMany(x => mechDef.Chassis.GetLocationDef(x).Hardpoints).ToArray();

                var calc = new HardpointOmniUsageCalculator(inventory, hardpoints);

                Control.Logger.Debug?.Log(calc);

                __instance.totalBallisticHardpoints = calc.Ballistic.TheoreticalMax;
                __instance.totalEnergyHardpoints = calc.Energy.TheoreticalMax;
                __instance.totalMissileHardpoints = calc.Missile.TheoreticalMax;
                __instance.totalSmallHardpoints = calc.Small.TheoreticalMax;

                static void SetData(MechLabHardpointElement element, HardpointStat stat)
                {
                    element.gameObject.SetActive(stat.ShowStat);
                    element.SetData(stat.CategoryForLocationWidget, stat.HardpointStringWithSpace);
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