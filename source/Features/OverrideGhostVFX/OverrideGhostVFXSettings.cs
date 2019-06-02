namespace MechEngineer.Features.OverrideGhostVFX
{
    internal class OverrideGhostVFXSettings : BaseSettings
    {
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