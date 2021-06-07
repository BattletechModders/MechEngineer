namespace MechEngineer.Features.OmniSlots
{
    public class OmniSlotsSettings : ISettings
    {
        public bool Enabled { get; set; } = false;
        public string EnabledDescription => "Enables the use of omni slots, where one can use any type of hardpoint for each omni slot.";
    }
}