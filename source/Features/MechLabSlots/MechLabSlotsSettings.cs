namespace MechEngineer.Features.MechLabSlots;

internal class MechLabSlotsSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Makes the mech lab adhere to any custom mech slot counts.";

    public bool HideHelpButton = false;
    public bool HideECMButton = false;

    public float ZoomMaximumScale = 0.95f;
    public string ZoomMaximumScaleDescription => "Set the minimum zoom, makes sure custom capacities fit.";

    public float MechLabStatusLeftShift = 0;
    public string MechLabStatusLeftShiftDescription => "Between 0 (vanilla) and 10. Allows custom capacities to be wider.";

    public float MechLabMechRightShift = 0;
    public string MechLabMechRightShiftDescription => "Between 0 (vanilla) and 30. Allows custom capacities to be wider.";

    public int MechLabArmTopPadding = 73;
    public string MechLabArmTopPaddingDescription =>
        "Optimal numbers" +
        ": 120 - no melee slot, max 4 top left widget slots, large overlap on bottom" +
        "; 95 - with melee slot, max 4 top left widget slots, large overlap on bottom" +
        "; 65 - with melee slot, max 3 top left widget slots, no overlap on bottom";

    public WidgetSettings TopLeftWidget = new()
    {
        Label = "Technology Base",
        ShortLabel = "TB",
        Slots = 3 // max 4
    };

    public WidgetSettings TopRightWidget = new()
    {
        Label = "Quirk",
        ShortLabel = "Q",
        Slots = 1 // max 5
    };

    internal class WidgetSettings
    {
        public string Label = null!;
        public string ShortLabel = null!;
        public int Slots;
        public bool Enabled => Slots > 0;
    }
}