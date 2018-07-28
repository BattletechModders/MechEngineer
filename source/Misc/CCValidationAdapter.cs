using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    internal class CCValidationAdapter
    {
        private readonly IValidateMech validator;
        public CCValidationAdapter(IValidateMech validator)
        {
            this.validator = validator;
        }

        public void ValidateMech(Dictionary<MechValidationType, List<string>> errors, MechValidationLevel validationLevel, MechDef mechDef)
        {
            validator.ValidateMech(mechDef, errors);
        }

        public bool ValidateMechCanBeFielded(MechDef mechDef)
        {
            var errors = (Dictionary<MechValidationType, List<string>>)
                Traverse.Create<MechValidationRules>()
                    .Method("InitializeValidationResults")
                    .GetValue();

            validator.ValidateMech(mechDef, errors);

            return errors[MechValidationType.InvalidInventorySlots].Count == 0;
        }
    }
}
