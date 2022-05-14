namespace MechEngineer.Features.CustomCapacities;

public class CustomCapacitiesSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Enables some carry rules.";

    public string ErrorOverweight = "OVERWEIGHT: 'Mechs carry weight exceeds maximum.";
    public string ErrorOneFreeHand = "OVERWEIGHT: 'Mechs carry weight requires one free hand.";

    public string CarryWeightLabel = "Lifting";

    // TODO implement UI, best something new inside hardpoint list or above
    // TODO find a way to set carry weights for arm actuators (right now all are just set to 1 usage)
}
