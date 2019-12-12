using BattleTech;
using Harmony;

namespace MechEngineer.Features.ComponentExplosions
{
    internal static class TurretPrivateExtensions
    {
        internal static bool DamageLocation(this Turret turret, WeaponHitInfo hitInfo, BuildingLocation bLoc, Weapon weapon, float totalArmorDamage, float directStructureDamage)
        {
            return Traverse.Create(turret).Method(nameof(DamageLocation), hitInfo, bLoc, weapon, totalArmorDamage, directStructureDamage).GetValue<bool>();
        }
    }
}