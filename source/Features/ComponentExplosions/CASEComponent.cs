using CustomComponents;

namespace MechEngineer.Features.ComponentExplosions;

[CustomComponent("CASE")]
public class CASEComponent : SimpleCustomComponent
{
    public float? MaximumDamage { get; set; } = null;
    public bool AllLocations { get; set; } = false;
}