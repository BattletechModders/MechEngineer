namespace MechEngineer.Features.ComponentExplosions;

internal interface IExplosionProtectionLocation
{
    public float? MaximumDamage { get; }
    public bool AllLocations { get; }
}