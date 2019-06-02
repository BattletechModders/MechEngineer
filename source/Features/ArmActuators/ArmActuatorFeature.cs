
using CustomComponents;

namespace MechEngineer.Features.ArmActuators
{
    internal class ArmActuatorFeature: Feature
    {
        internal static ArmActuatorFeature Shared = new ArmActuatorFeature();

        internal override bool Enabled => settings.Enabled;

        internal static Settings settings => Control.settings.ArmActuator;

        internal override void SetupFeatureLoaded()
        {
            Validator.RegisterClearInventory(ArmActuatorCC.ClearInventory);

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

        public class Settings
        {
            public bool Enabled = true;
            public bool ForceFullDefaultActuators = false;
            public string IgnoreFullActuatorsTag = null;
            public string DefaultCBTShoulder = "emod_arm_part_shoulder";
            public string DefaultCBTLower = "emod_arm_part_lower";
            public string DefaultCBTUpper = "emod_arm_part_upper";
            public string DefaultCBTHand = "emod_arm_part_hand";
            public string DefaultCBTDefLower = "emod_arm_part_lower";
            public string DefaultCBTDefHand = "emod_arm_part_hand";
            public bool InterruptHandDropIfNoLower = false;
            public bool ExtendHandLimit = true;
        }
    }
}