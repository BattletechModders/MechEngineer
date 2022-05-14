namespace MechEngineer.Features.CustomCapacities;

public class CustomCapacitiesSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Enables some carry rules.";

    public string CarryHandErrorOverweight = "OVERWEIGHT: 'Mechs handheld carry weight exceeds maximum.";
    public string CarryHandErrorOneFreeHand = "OVERWEIGHT: 'Mechs handheld carry weight requires one free hand.";

    public string CarryTotalErrorOverweight = "OVERWEIGHT: 'Mechs total carry weight exceeds maximum.";
    public string CarryTotalLabel = "Carry Weight";
}
