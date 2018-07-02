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
        }

        internal static MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, List<MechLabItemSlotElement> localInventory)
        {
            var validators = new IValidateDrop[]
            {
                LegUpgradeHandler.Shared,
                EngineSideDefHandler.Shared,
                EngineCoreDefHandler.Shared,
                EngineTypeDefHandler.Shared,
                GyroHandler.Shared,
                CockpitHandler.Shared,
            };

            foreach (var validator in validators)
            {
                var result = validator.ValidateDrop(dragItem, localInventory);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}