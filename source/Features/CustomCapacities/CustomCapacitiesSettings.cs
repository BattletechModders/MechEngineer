namespace MechEngineer.Features.CustomCapacities
{
    public class CustomCapacitiesSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Enables some HandHeld Weapons and TSM rules.";

        public string ErrorOverweight = "OVERWEIGHT: 'Mechs carry weight exceeds maximum achieved with installed hand actuators.";
        public string LocationLabel = "HandHeld {0:0.00}/{1:0.00}t";
    }
}
