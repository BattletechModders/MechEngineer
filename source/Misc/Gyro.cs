using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    internal static class Gyro
    {
        // allow gyro upgrades to be 1 slot and still only be added once
        internal static void ValidateAdd(
            MechComponentDef newComponentDef,
            List<MechLabItemSlotElement> localInventory,
            ref string dropErrorMessage,
            ref bool result)
        {
            try
            {
                if (!result)
                {
                    return;
                }

                if (!newComponentDef.IsCenterTorsoUpgrade())
                {
                    return;
                }
                
                if (localInventory.Select(x => x.ComponentRef).All(x => x == null || !x.Def.IsCenterTorsoUpgrade()))
                {
                    return;
                }

                dropErrorMessage = String.Format("Cannot add {0}: A center torso upgrade is already installed", newComponentDef.Description.Name);
                result = false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        // reduce upgrade components for the center torso that are 3 or larger 
        internal static void ReduceGyroSize(UpgradeDef upgradeDef)
        {
            try
            {
                if (!upgradeDef.IsCenterTorsoUpgrade())
                {
                    return;
                }

                if (upgradeDef.InventorySize < 3)
                {
                    return;
                }

                var value = upgradeDef.InventorySize - 2;
                var propInfo = typeof(UpgradeDef).GetProperty("InventorySize");
                var propValue = Convert.ChangeType(value, propInfo.PropertyType);
                propInfo.SetValue(upgradeDef, propValue, null);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}