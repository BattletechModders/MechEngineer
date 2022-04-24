namespace MechEngineer.Features.OverrideGhostVFX;

internal class OverrideGhostVFXSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Allows to turn off or reduce the strong and weak ghost effects of ECM, and allows to remove the ECM globe effect.";

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