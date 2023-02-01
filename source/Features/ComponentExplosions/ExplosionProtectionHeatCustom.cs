using CustomComponents;

namespace MechEngineer.Features.ComponentExplosions;

[CustomComponent("ExplosionProtectionHeat")]
internal class ExplosionProtectionHeatCustom : SimpleCustomComponent, IExplosionProtectionLocation
{
    public float? MaximumDamage { get; set; } = null;
    public bool AllLocations { get; set; } = false;
}