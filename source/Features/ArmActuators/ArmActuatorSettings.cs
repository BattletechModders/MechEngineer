namespace MechEngineer.Features.ArmActuators
{
    public class ArmActuatorSettings : BaseSettings
    {
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