namespace MechEngineer.Features.OverrideGhostVFX;

internal class OverrideGhostVFXFeature : Feature<OverrideGhostVFXSettings>
{
    internal static readonly OverrideGhostVFXFeature Shared = new();

    internal override OverrideGhostVFXSettings Settings => Control.Settings.OverrideGhostVFX;
}