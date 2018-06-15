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

                if (!newComponentDef.IsGyro())
                {
                    return;
                }
                
                if (localInventory.Select(x => x.ComponentRef).All(x => x == null || !x.Def.IsGyro()))
                {
                    return;
                }

                dropErrorMessage = String.Format("Cannot add {0}: A gyro is already installed", newComponentDef.Description.Name);
                result = false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        // reduce upgrade components for the center torso that are 3 or larger 
        internal static void AdjustGyroUpgrade(UpgradeDef upgradeDef)
        {
            try
            {
                if (!Control.settings.AutoFixGyroUpgrades)
                {
                    return;
                }

                if (!upgradeDef.IsGyro())
                {
                    return;
                }

                if (upgradeDef.InventorySize != 3)
                {
                    return;
                }

                var value = 4;
                var propInfo = typeof(UpgradeDef).GetProperty("InventorySize");
                var propValue = Convert.ChangeType(value, propInfo.PropertyType);
                propInfo.SetValue(upgradeDef, propValue, null);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            if (mechDef.Inventory.Any(x => x.Def != null && x.Def.IsGyro()))
            {
                return;
            }

            errorMessages[MechValidationType.InvalidInventorySlots].Add("MISSING GYRO: This Mech must mount a gyro");
        }

        internal static void AddGyroIfPossible(MechDef mechDef)
        {
            if (!Control.settings.AutoFixMechDefGyro)
            {
                return;
            }

            if (mechDef.Inventory.Any(x => x.Def != null && x.Def.IsGyro()))
            {
                return;
            }

            var componentRefs = new List<MechComponentRef>(mechDef.Inventory);

            var componentRef = new MechComponentRef(Control.settings.AutoFixMechDefGyroId, null, ComponentType.Upgrade, ChassisLocations.CenterTorso);
            componentRefs.Add(componentRef);

            mechDef.SetInventory(componentRefs.ToArray());
        }
    }
}