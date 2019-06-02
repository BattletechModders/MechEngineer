namespace MechEngineer.Features.DynamicSlots
{
    public class DynamicSlotsSettings : BaseSettings
    {
        // MWO does not allow to drop if that would mean to go overweight
        // battletech allows overweight, to stay consistent so we also allow overspace usage by default
        // set to true to switch to MWO style
        public bool DynamicSlotsValidateDropEnabled = false;
    }
}