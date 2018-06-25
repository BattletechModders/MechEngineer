using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    internal static class Cockpit
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

                if (!newComponentDef.IsCockpit())
                {
                    return;
                }
                
                if (localInventory.Select(x => x.ComponentRef).All(x => x == null || !x.Def.IsCockpit()))
                {
                    return;
                }

                dropErrorMessage = String.Format("Cannot add {0}: A cockpit is already installed", newComponentDef.Description.Name);
                result = false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        // reduce upgrade components for the center torso that are 3 or larger 
        internal static void AdjustCockpitUpgrade(UpgradeDef upgradeDef)
        {
            try
            {
                if (!Control.settings.AutoFixCockpitUpgrades)
                {
                    return;
                }

                if (!upgradeDef.IsCockpit())
                {
                    return;
                }

                if (upgradeDef.Tonnage > 0.1)
                {
                    return;
                }

                var value = 3;
                var propInfo = typeof(UpgradeDef).GetProperty("Tonnage");
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
            if (mechDef.Inventory.Any(x => x.Def != null && x.Def.IsCockpit()))
            {
                return;
            }

            errorMessages[MechValidationType.InvalidInventorySlots].Add("MISSING COCKPIT: This Mech must mount a cockpit");
        }

        internal static void AddCockpitIfPossible(MechDef mechDef)
        {
            if (!Control.settings.AutoFixMechDefCockpit)
            {
                return;
            }

            if (mechDef.Inventory.Any(x => x.Def != null && x.Def.IsCockpit()))
            {
                return;
            }

            var componentRefs = new List<MechComponentRef>(mechDef.Inventory);

            var componentRef = new MechComponentRef(Control.settings.AutoFixMechDefCockpitId, null, ComponentType.Upgrade, ChassisLocations.Head);
            componentRefs.Add(componentRef);

            mechDef.SetInventory(componentRefs.ToArray());
        }
    }
}