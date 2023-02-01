using CustomComponents;

namespace MechEngineer.Features.ComponentExplosions;

[CustomComponent("CASE")]
internal class ExplosionProtectionStructureCustom : SimpleCustomComponent, IExplosionProtectionLocation
{
    public float? MaximumDamage { get; set; } = null;
    public bool AllLocations { get; set; } = false;
}