namespace MechEngineer.Features.ArmActuators
{
    public class ArmActuatorSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Enables CBT arm actuators with CC like handling.";

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

        public string DescriptionIdentifier = "ArmActuatorSupport";
        public string DescriptionTemplate = "<color=#F79B26FF><b>Arm Actuator Limit</b>: <b>{0}</b> on left and <b>{1}</b> on right</color>\r\n";
        public string CustomDescriptionTemplate = "<color=#F79B26FF><b>Arm Actuator Limit</b>: {0}</color>\r\n";
    }
}