namespace MechEngineer.Features.MechLabSlots;

internal class MechLabSlotsSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Makes the mech lab adhere to any custom mech slot counts.";

    public bool HideHelpButton = false;
    public bool HideECMButton = false;

    public int MechLabArmTopPadding = 120;

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