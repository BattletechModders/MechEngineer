using CustomComponents;

namespace MechEngineer.Features.ComponentExplosions;

[CustomComponent("ComponentExplosion")]
public class ComponentExplosion : SimpleCustomComponent
{
    public float ExplosionDamage { get; set; }
    public float ExplosionDamagePerAmmo { get; set; }
    public float HeatDamage { get; set; }
    public float HeatDamagePerAmmo { get; set; }
    public float StabilityDamage { get; set; }
    public float StabilityDamagePerAmmo { get; set; }
}