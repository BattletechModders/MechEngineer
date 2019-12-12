using BattleTech;
using Harmony;

namespace MechEngineer.Features.ComponentExplosions
{
    internal static class MechPrivateExtensions
    {
        internal static bool DamageLocation(this Mech mech, int originalHitLoc, WeaponHitInfo hitInfo, ArmorLocation aLoc, Weapon weapon, float totalArmorDamage, float directStructureDamage, int hitIndex, AttackImpactQuality impactQuality, DamageType damageType)
        {
            return Traverse.Create(mech).Method(nameof(DamageLocation), originalHitLoc, hitInfo, aLoc, weapon, totalArmorDamage, directStructureDamage, hitIndex, impactQuality, damageType).GetValue<bool>();
        }
    }
}