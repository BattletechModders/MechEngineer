namespace MechEngineer.Features.CustomCapacities;

public class CustomCapacitiesSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Enables some carry rules.";

    public string CarryHandErrorOverweight = "OVERWEIGHT: 'Mechs handheld carry weight exceeds maximum.";
    public string CarryHandErrorOneFreeHand = "OVERWEIGHT: 'Mechs handheld carry weight requires one free hand.";
    public string CarryTotalErrorOverweight = "OVERWEIGHT: 'Mechs total carry weight exceeds maximum.";

    public string CarryTotalLabel = "Carry Weight";
    public string CarryTotalFormat = "{0:0.0} / {1:0.0}";

    public CustomCapacity[] CustomCapacities =
    {
        new()
        {
            Collection = "Special",
            Label = "e.g. Special",
            Format = "{0:0} / {1:0}",
            ErrorOverweight = "OVERUSE: 'Mechs special points exceeds maximum.",
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

        public bool HideIfNoUsageAndCapacity { get; set; }
        public string HideIfNoUsageAndCapacityDescription => "Hides the capacity if usage and capacity amounts are 0.";
    }
}
