using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    internal static class LegUpgrade
    {
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

                if (!newComponentDef.IsLegUpgrade())
                {
                    return;
                }
                
                if (localInventory.Select(x => x.ComponentRef).All(x => x == null || !x.Def.IsLegUpgrade()))
                {
                    return;
                }

                dropErrorMessage = String.Format("Cannot add {0}: A leg actuator is already installed", newComponentDef.Description.Name);
                result = false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        // reduce upgrade components for the center torso that are 3 or larger 
        internal static void AdjustLegUpgrade(UpgradeDef upgradeDef)
        {
            try
            {
                if (!Control.settings.AutoFixLegUpgrades)
                {
                    return;
                }

                if (!upgradeDef.IsLegUpgrade())
                {
                    return;
                }

                if (upgradeDef.InventorySize != 3)
                {
                    return;
                }

                var value = 1;
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