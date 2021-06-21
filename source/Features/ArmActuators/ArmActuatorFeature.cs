
using CustomComponents;

namespace MechEngineer.Features.ArmActuators
{
    internal class ArmActuatorFeature: Feature<ArmActuatorSettings>
    {
        internal static ArmActuatorFeature Shared = new();

        internal override ArmActuatorSettings Settings => new();

        internal static ArmActuatorSettings settings => Shared.Settings;

        internal override void SetupFeatureLoaded()
        {
            //!TODO PONE FIX IT
            //Validator.RegisterClearInventory(ArmActuatorCC.ClearInventory);

            if (settings.ForceFullDefaultActuators)
            {
                Validator.RegisterMechValidator(ArmActuatorCC.ValidateMechFF, ArmActuatorCC.CanBeFieldedFF);

                AutoFixer.Shared.RegisterMechFixer(ArmActuatorCC.FixCBTActuatorsFF);
            }
            else
            {
                Validator.RegisterMechValidator(ArmActuatorCC.ValidateMech, ArmActuatorCC.CanBeFielded);

                AutoFixer.Shared.RegisterMechFixer(ArmActuatorCC.FixCBTActuators);
            }
        }
    }
}