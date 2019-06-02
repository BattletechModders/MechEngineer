
namespace MechEngineer.Features.OverrideGhostVFX
{
    internal class OverrideGhostVFXFeature : Feature
    {
        internal static OverrideGhostVFXFeature Shared = new OverrideGhostVFXFeature();

        internal override bool Enabled => Control.settings.OverrideGhostVFX?.Enabled ?? false;

        internal class Settings
        {
            public bool Enabled = true;
            public string[] Blacklisted = {"vfxPrfPrtl_ECM_loop", "vfxPrfPrtl_ECM_opponent_loop", "vfxPrfPrtl_ECMcarrierAura_loop"};
            public BlipGhostType BlipWeak = BlipGhostType.Weak;
            public BlipGhostType BlipStrong = BlipGhostType.Weak;

            public enum BlipGhostType
            {
                None,
                Weak,
                Strong
            }
        }
    }
}
