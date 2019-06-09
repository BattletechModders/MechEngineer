using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Harmony;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.TurretLimitedAmmo;

namespace MechEngineer.Features.OmniSlots
{
    internal class OmniSlotsFeature: Feature<TurretLimitedAmmoSettings>, IValidateMech
    {
        internal static OmniSlotsFeature Shared = new OmniSlotsFeature();

        internal override TurretLimitedAmmoSettings Settings => new TurretLimitedAmmoSettings();

        internal override void SetupFeatureLoaded()
        {
            base.SetupFeatureLoaded();

            Validator.HardpointValidator = HardpointValidator;
            var ccValidator = new CCValidationAdapter(this);
            Validator.RegisterMechValidator(ccValidator.ValidateMech, ccValidator.ValidateMechCanBeFielded);
        }

        public void ValidateMech(MechDef mechDef, Errors errors)
        {
            foreach (var location in MechDefBuilder.Locations)
            {
                var inventory = mechDef.Inventory.Where(x => x.MountedLocation == location).Select(x => x.Def);
                var hardpoints = mechDef.Chassis.GetLocationDef(location).Hardpoints;

                var calc = new HardpointOmniUsageCalculator(inventory, hardpoints);

                if (calc.OmniFree < 0)
                {
                    if (errors.Add(MechValidationType.InvalidHardpoints, ErrorMessage(location)))
                    {
                        return;
                    }
                }
            }
        }

        private string ErrorMessage(ChassisLocations location)
        {
            var locationName = Mech.GetLongChassisLocation(location);
            return $"'Mech has too many weapons mounted in the {locationName}";
        }

        // TODO move to CC, probably a new thing that is a cross between ValidateMech + drop check
        internal bool ValidateAdd(
            bool dropCheck,
            ref List<MechLabItemSlotElement> localInventory,
            ref LocationDef chassisLocationDef,
            MechComponentDef newComponentDef)
        {
            if (newComponentDef.ComponentType != ComponentType.Weapon)
            {
                return true;
            }

            var inventory = localInventory.Select(x => x.ComponentRef.Def);
            var hardpoints = chassisLocationDef.Hardpoints;
            if (hardpoints == null)
            {
                // how can this happen? is this from the properties widget?
                //Control.mod.Logger.LogDebug($"hardpoints is null");
                return true;
            }
                
            var calc = new HardpointOmniUsageCalculator(inventory, hardpoints);
            return calc.CanAdd(dropCheck, newComponentDef);
        }

        private string HardpointValidator(MechLabItemSlotElement drop_item, LocationHelper locationHelper, List<IChange> changes)
        {
            var componentDef = drop_item.ComponentRef.Def;
            if (componentDef.ComponentType != ComponentType.Weapon)
            {
                return null;
            }

            var inventory = locationHelper.LocalInventory.Select(x => x.ComponentRef.Def);
            var location = locationHelper.widget.loadout.Location;
            var mechDef = locationHelper.mechLab.activeMechDef;
            var hardpoints = mechDef.Chassis.GetLocationDef(location).Hardpoints;

            var calc = new HardpointOmniUsageCalculator(inventory, hardpoints);

            if (calc.CanAdd(true, componentDef))
            {
                return null;
            }
            else
            {
                return ErrorMessage(location);
            }
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Property(typeof(MechComponentDef), nameof(MechComponentDef.ComponentType)).GetGetMethod(),
                AccessTools.Method(typeof(OmniSlotsFeature), nameof(GetComponentType))
            ).MethodReplacer(
                AccessTools.Property(typeof(BaseComponentRef), nameof(BaseComponentRef.ComponentDefType)).GetGetMethod(),
                AccessTools.Method(typeof(OmniSlotsFeature), nameof(GetComponentDefType))
            );
        }

        internal static ComponentType GetComponentType(MechComponentDef @this)
        {
            return @this.ComponentType == ComponentType.Weapon ? ComponentType.NotSet : @this.ComponentType;
        }

        internal static ComponentType GetComponentDefType(BaseComponentRef @this)
        {
            return @this.ComponentDefType == ComponentType.Weapon ? ComponentType.NotSet : @this.ComponentDefType;
        }
    }
}