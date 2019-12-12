using BattleTech;
using Harmony;

namespace MechEngineer.Features.ComponentExplosions
{
    internal static class VehiclePrivateExtensions
    {
        internal static void applyArmorStatDamage(this Vehicle vehicle, VehicleChassisLocations location, float damage, WeaponHitInfo hitInfo)
        {
            Traverse.Create(vehicle).Method(nameof(applyArmorStatDamage), location, damage, hitInfo).GetValue();
        }

        internal static void applyStructureStatDamage(this Vehicle vehicle, VehicleChassisLocations location, float damage, WeaponHitInfo hitInfo)
        {
            Traverse.Create(vehicle).Method(nameof(applyStructureStatDamage), location, damage, hitInfo).GetValue();
        }

        internal static bool DamageLocation(this Vehicle vehicle, WeaponHitInfo hitInfo, int originalHitLoc, VehicleChassisLocations vLoc, Weapon weapon, float totalArmorDamage, float directStructureDamage, AttackImpactQuality impactQuality)
        {
            return Traverse.Create(vehicle).Method(nameof(DamageLocation), hitInfo, originalHitLoc, vLoc, weapon, totalArmorDamage, directStructureDamage, impactQuality).GetValue<bool>();
        }
    }
}