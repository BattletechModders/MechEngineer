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
            EngineSlotsCenterHandler.Shared.ValidateMech(mechDef, errorMessages);
            GyroHandler.Shared.ValidateMech(mechDef, errorMessages);
            CockpitHandler.Shared.ValidateMech(mechDef, errorMessages);
        }

        internal static void ValidateAdd(
            MechComponentDef newComponentDef,
            List<MechLabItemSlotElement> localInventory,
            ref string ___dropErrorMessage,
            ref bool __result)
        {
            var validators = new IValidateAdd[]
            {
                GyroHandler.Shared,
                LegUpgradeHandler.Shared,
                EngineSlotsHandler.Shared,
                EngineCoreDefHandler.Shared,
                CockpitHandler.Shared
            };
            foreach (var validator in validators)
            {
                validator.ValidateAdd(newComponentDef, localInventory, ref ___dropErrorMessage, ref __result);
                if (!__result)
                {
                    return;
                }
            }
        }
    }
}