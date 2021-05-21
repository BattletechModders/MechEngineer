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

                ___hardpoints[0].SetData(WeaponCategoryEnumeration.GetBallistic(), calc.Ballistic.HardpointStringWithSpace);
                ___hardpoints[1].SetData(WeaponCategoryEnumeration.GetEnergy(), calc.Energy.HardpointStringWithSpace);
                ___hardpoints[2].SetData(WeaponCategoryEnumeration.GetMissile(), calc.Missile.HardpointStringWithSpace);
                ___hardpoints[3].SetData(WeaponCategoryEnumeration.GetSupport(), calc.Small.HardpointStringWithSpace);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}