namespace MechEngineer.Features.DynamicSlots
{
    public class DynamicSlotsSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Allows components to take up space dynamically on a mech.";

        // MWO does not allow to drop if that would mean to go overweight
        // battletech allows overweight, to stay consistent so we also allow overspace usage by default
        // set to true to switch to MWO style
        public bool DynamicSlotsValidateDropEnabled = false;
        public string DynamicSlotsValidateDropEnabledDescription = "Don't allow dropping of items that would exceed the available slots.";
    }
}