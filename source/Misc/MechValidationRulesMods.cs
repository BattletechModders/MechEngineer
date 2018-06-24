using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleTech;

namespace MechEngineMod
{
    internal static class MechValidationRulesMods
    {
        public static void Validate(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            ArmorStructure.ValidationRulesCheck(mechDef, ref errorMessages);
            EngineHeat.ValidationRulesCheck(mechDef, ref errorMessages);
            EngineMisc.ValidationRulesCheck(mechDef, ref errorMessages);
            Gyro.ValidationRulesCheck(mechDef, ref errorMessages);
            Cockpit.ValidationRulesCheck(mechDef, ref errorMessages);
        }
    }
}
