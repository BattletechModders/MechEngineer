using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using MechEngineer.Features.DynamicSlots;

namespace MechEngineer.Features.OmniSlots.Patches
{
    [HarmonyPatch(typeof(SG_Shop_ItemSelectedPanel), nameof(SG_Shop_ItemSelectedPanel.FillInHardPoints))]
    internal static class SG_Shop_ItemSelectedPanel_FillInHardPoints_Patch
    {
        internal static void Postfix(
            MechDef mechDef,
            MechLabHardpointElement ___BallisticHardPointElement,
            MechLabHardpointElement ___EnergyHardPointElement,
            MechLabHardpointElement ___MissileHardPointElement,
            MechLabHardpointElement ___SmallHardPointElement)
        {
            try
            {
                if (mechDef == null)
                {
                    return;
                }
                
                var inventory = mechDef.Inventory.Select(x => x.Def);
                var hardpoints = MechDefBuilder.Locations.SelectMany(x => mechDef.Chassis.GetLocationDef(x).Hardpoints).ToArray();

                var calc = new HardpointOmniUsageCalculator(inventory, hardpoints);

                //Control.mod.Logger.Log(calc);

                ___BallisticHardPointElement.SetData(WeaponCategory.Ballistic, calc.Ballistic.HardpointString);
                ___EnergyHardPointElement.SetData(WeaponCategory.Energy, calc.Energy.HardpointString);
                ___MissileHardPointElement.SetData(WeaponCategory.Missile, calc.Missile.HardpointString);
                ___SmallHardPointElement.SetData(WeaponCategory.AntiPersonnel, calc.Small.HardpointString);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}