using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    internal class ArmActuatorHandler : IAdjustUpgradeDef, IAutoFixMechDef, IPreProcessor, IValidateMech
    {
        internal static ArmActuatorHandler Shared = new ArmActuatorHandler();
        internal CCValidationAdapter CCValidation;
        
        private readonly IdentityHelper identity;
        private readonly AdjustCompDefInvSizeHelper resizer;
        

        private ArmActuatorHandler()
        {
            identity = Control.settings.AutoFixArmActuatorCategorizer;
            
            CCValidation = new CCValidationAdapter(this);
            resizer = new AdjustCompDefInvSizeHelper(identity, Control.settings.AutoFixArmActuatorSlotChange);
        }

        public void PreProcess(object target, Dictionary<string, object> values)
        {
            identity.PreProcess(target, values);
        }

        public void AdjustUpgradeDef(UpgradeDef upgradeDef)
        {
            resizer.AdjustComponentDef(upgradeDef);
        }

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            //Control.mod.Logger.LogDebug($"ArmActuatorHandler.AutoFixMechDef");
            //Control.mod.Logger.LogDebug($" chassis={mechDef.Chassis.Description.Id}");

            var builder = new MechDefBuilder(mechDef.Chassis, mechDef.Inventory.ToList());
            UpgradeDef GetComponent(ArmActuator.TypeDef type)
            {
                switch (type)
                {
                    case ArmActuator.TypeDef.Hand:
                        return mechDef.DataManager.UpgradeDefs.Get("emod_arm_hand");
                    case ArmActuator.TypeDef.Lower:
                        return mechDef.DataManager.UpgradeDefs.Get("emod_arm_lower");
                    case ArmActuator.TypeDef.Upper:
                        return mechDef.DataManager.UpgradeDefs.Get("emod_arm_upper");
                    default:
                        return null;
                }
            }

            void AddActuatorToArm(ChassisLocations location, ArmActuator.TypeDef limit)
            {
                if (mechDef.Inventory.Any(r => r.MountedLocation == location && r.Def.Is<ArmActuator>()))
                {
                    return;
                }

                //Control.mod.Logger.LogDebug($" AddActuatorToArm");
                //Control.mod.Logger.LogDebug($" location={location} limit={limit}");
                foreach (var candidate in Enum.GetValues(typeof(ArmActuator.TypeDef)).Cast<ArmActuator.TypeDef>().Where(type => type >= limit))
                {
                    //Control.mod.Logger.LogDebug($"  candidate={candidate}");
                    var component = GetComponent(candidate);
                    if (candidate <= ArmActuator.TypeDef.Upper && builder.GetFreeSlots(location) < component.InventorySize)
                    {
                        continue;
                    }

                    //Control.mod.Logger.LogDebug($"  add");
                    builder.Add(component, location);
                    break;
                }
            }

            var limits = mechDef.Chassis.GetComponent<ArmActuatorSupport>() ?? new ArmActuatorSupport();
            var beforeCount = builder.Inventory.Count;
            AddActuatorToArm(ChassisLocations.LeftArm, limits.LeftLimit);
            AddActuatorToArm(ChassisLocations.RightArm, limits.RightLimit);
            if (builder.Inventory.Count != beforeCount)
            {
                mechDef.SetInventory(builder.Inventory.ToArray());
            }
        }

        public void ValidateMech(MechDef mechDef, Errors errors)
        {
            var left = mechDef.Inventory.Where(r => r.MountedLocation == ChassisLocations.LeftArm).Select(r => r.GetComponent<ArmActuator>()).FirstOrDefault(r => r != null);
            var right = mechDef.Inventory.Where(r => r.MountedLocation == ChassisLocations.RightArm).Select(r => r.GetComponent<ArmActuator>()).FirstOrDefault(r => r != null);

            var limits = mechDef.Chassis.GetComponent<ArmActuatorSupport>() ?? new ArmActuatorSupport();

            //Control.mod.Logger.LogDebug($"left={left} limits.LeftLimit={limits.LeftLimit}");

            if (left != null && left.Type < limits.LeftLimit)
            {
                if (errors.Add(MechValidationType.InvalidInventorySlots, $"ARM ACTUATOR: Left arm only supports down to an {limits.LeftLimit} actuator."))
                {
                    return;
                }
            }
            
            //Control.mod.Logger.LogDebug($"right={right} limits.RightLimit={limits.RightLimit}");

            if (right != null && right.Type < limits.RightLimit)
            {
                if (errors.Add(MechValidationType.InvalidInventorySlots, $"ARM ACTUATOR: Right arm only supports down to an {limits.RightLimit} actuator."))
                {
                    return;
                }
            }
        }
    }
}