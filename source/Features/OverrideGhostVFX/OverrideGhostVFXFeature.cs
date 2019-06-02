
namespace MechEngineer.Features.OverrideGhostVFX
{
    internal class OverrideGhostVFXFeature : Feature<OverrideGhostVFXSettings>
    {
        internal static OverrideGhostVFXFeature Shared = new OverrideGhostVFXFeature();

        internal override OverrideGhostVFXSettings Settings => Control.settings.OverrideGhostVFX;
    }
}
