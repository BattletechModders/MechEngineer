namespace MechEngineer.Features.CustomCapacities
{
    public class CustomCapacitiesSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Enables some carry rules.";

        public string ErrorOverweight = "OVERWEIGHT: 'Mechs carry weight exceeds maximum achieved with installed hand actuators.";
        public string ErrorOneFreeHand = "OVERWEIGHT: 'Mechs carry weight requires one free hand.";

        // TODO implement UI, best something new inside hardpoint list or above
        // TODO implement status effect templating to easily fix arm actuator dmg mods
        // TODO move all arm actuators to same mechanism as the hachet
        // TODO normalize melee upgrade damage: TSM, ArmActuator upgrades, Hatchet
        public string LocationLabel = "HandHeld {0:0.00}/{1:0.00}t";
    }
}
