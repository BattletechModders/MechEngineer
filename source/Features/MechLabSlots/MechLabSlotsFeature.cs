namespace MechEngineer.Features.MechLabSlots;

internal class MechLabSlotsFeature : Feature<MechLabSlotsSettings>
{
    internal static readonly MechLabSlotsFeature Shared = new();

    internal override MechLabSlotsSettings Settings => Control.Settings.MechLabSlots;

    internal static MechLabSlotsSettings settings => Shared.Settings;
}