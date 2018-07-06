using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal static class ValidationFacade
    {
        internal static void ValidateMech(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            ArmorHandler.Shared.ValidateMech(mechDef, errorMessages);
            StructureHandler.Shared.ValidateMech(mechDef, errorMessages);
            EngineCoreRefHandler.Shared.ValidateMech(mechDef, errorMessages);
            EngineCoreDefHandler.Shared.ValidateMech(mechDef, errorMessages);
            EngineTypeDefHandler.Shared.ValidateMech(mechDef, errorMessages);
            GyroHandler.Shared.ValidateMech(mechDef, errorMessages);
            CockpitHandler.Shared.ValidateMech(mechDef, errorMessages);
            DynamicSlotController.Shared.ValidateMech(mechDef, errorMessages);
        }

        internal static MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, MechLabLocationWidget widget)
        {
            var validators = new IValidateDrop[]
            {
                EngineHeat.Shared,
                ArmorHandler.Shared,
                StructureHandler.Shared,
                LegUpgradeHandler.Shared,
                EngineSideDefHandler.Shared,
                EngineCoreDefHandler.Shared,
                EngineTypeDefHandler.Shared,
                GyroHandler.Shared,
                CockpitHandler.Shared,
                //DynamicSlotController.Shared // we dont want the drop check, we allow over use of inventory until save (similar to overtonnage)
            };

            foreach (var validator in validators)
            {
                var result = validator.ValidateDrop(dragItem, widget);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}