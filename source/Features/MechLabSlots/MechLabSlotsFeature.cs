namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabSlotsFeature : Feature<MechLabSlotsSettings>
    {
        internal static MechLabSlotsFeature Shared = new MechLabSlotsFeature();

        internal override MechLabSlotsSettings Settings => Control.settings.MechLabSlots;

        internal static MechLabSlotsSettings settings => Shared.Settings;
    }
}
