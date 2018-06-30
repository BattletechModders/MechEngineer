using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal static class ValidationFacade
    {
        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            ArmorHandler.Shared.ValidationRulesCheck(mechDef, errorMessages);
            StructureHandler.Shared.ValidationRulesCheck(mechDef, errorMessages);
            EngineCoreRefHandler.Shared.ValidationRulesCheck(mechDef, errorMessages);
            EngineCoreDefHandler.Shared.ValidationRulesCheck(mechDef, errorMessages);
            EngineSlotsCenterHandler.Shared.ValidationRulesCheck(mechDef, errorMessages);
            GyroHandler.Shared.ValidationRulesCheck(mechDef, errorMessages);
            CockpitHandler.Shared.ValidationRulesCheck(mechDef, errorMessages);
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