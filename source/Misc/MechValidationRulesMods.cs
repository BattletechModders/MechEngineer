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
            Structure.ValidationRulesCheck(mechDef, ref errorMessages);
            Armor.ValidationRulesCheck(mechDef, ref errorMessages);
            EngineHeat.ValidationRulesCheck(mechDef, ref errorMessages);
            Engine.ValidationRulesCheck(mechDef, ref errorMessages);
            Gyro.ValidationRulesCheck(mechDef, ref errorMessages);
            Cockpit.ValidationRulesCheck(mechDef, ref errorMessages);
        }
    }
}
