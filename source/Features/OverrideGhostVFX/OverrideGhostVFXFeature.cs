
namespace MechEngineer.Features.OverrideGhostVFX
{
    internal class OverrideGhostVFXFeature : Feature
    {
        internal static OverrideGhostVFXFeature Shared = new OverrideGhostVFXFeature();

        internal override bool Enabled => Control.settings.FeatureOverrideGhostVFX?.Enabled ?? false;
    }
}
