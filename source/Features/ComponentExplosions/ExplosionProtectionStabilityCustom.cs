using CustomComponents;

namespace MechEngineer.Features.ComponentExplosions;

[CustomComponent("ExplosionProtectionStability")]
internal class ExplosionProtectionStabilityCustom : SimpleCustomComponent, IExplosionProtectionLocation
{
    public float? MaximumDamage { get; set; } = null;
    public bool AllLocations { get; set; } = false;
}