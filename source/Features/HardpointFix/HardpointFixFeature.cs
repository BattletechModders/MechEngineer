
namespace MechEngineer.Features.HardpointFix
{
    internal class HardpointFixFeature : Feature<HardpointFixSettings>
    {
        internal static HardpointFixFeature Shared = new HardpointFixFeature();

        internal override HardpointFixSettings Settings => Control.settings.HardpointFix;
    }
}
