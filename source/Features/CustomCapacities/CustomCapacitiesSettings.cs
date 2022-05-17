namespace MechEngineer.Features.CustomCapacities;

public class CustomCapacitiesSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Enables some carry rules.";

    public string CarryHandErrorOverweight = "OVERWEIGHT: 'Mechs handheld carry weight exceeds maximum.";
    public string CarryHandErrorOneFreeHand = "OVERWEIGHT: 'Mechs handheld carry weight requires one free hand.";

    public CustomCapacity CarryWeight = new()
    {
        Collection = "carry_weight",
        Label = "Carry Weight",
        Format = "0.0",
        ErrorOverweight = "OVERWEIGHT: 'Mechs total carry weight exceeds maximum.",
        ToolTipHeader = "Carry Weight",
        ToolTipBody = "Carry weight represents the total carry capacity of a mech on top of the normal chassis weight internal capacity." +
                      " Each hand actuator allows to carry an equivalent of up to 5% chassis maximum tonnage." +
                      " If a melee weapon is too heavy for a single arm, it can be held two-handed by combining both hands carry capacities.",
        HideIfNoUsageAndCapacity = true
    };

    public CustomCapacity[] CustomCapacities =
    {
        new()
        {
            Collection = "Special",
            Label = "e.g. Special",
            Format = "0",
            ErrorOverweight = "OVERUSE: 'Mechs special points exceeds maximum.",
            ToolTipHeader = "Special Points",
            ToolTipBody = "This is just an example on how you can define custom capacities on anything and use it up on anything." +
                          " Useful if you want some more knapsack problem solving gameplay.",
            HideIfNoUsageAndCapacity = true
        }
    };

    public class CustomCapacity
    {
        public string Collection { get; set; } = null!;
        public string CollectionDescription => "The collection id that is referenced from the CapacityMod custom";

        public string Label { get; set; } = null!;
        public string Format { get; set; } = null!;
        public string ErrorOverweight { get; set; } = null!;

        public string? ToolTipHeader { get; set; }
        public string? ToolTipBody { get; set; }

        public bool HideIfNoUsageAndCapacity { get; set; }
        public string HideIfNoUsageAndCapacityDescription => "Hides the capacity if usage and capacity amounts are 0.";
    }
}
