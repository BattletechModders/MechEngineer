using System;
using System.Collections.Generic;
using BattleTech;
using MechEngineer.Helper;
using UnityEngine;

namespace MechEngineer.Features.ComponentExplosions.Patches;

[HarmonyPatch(typeof(Vehicle), nameof(Vehicle.DamageLocation))]
internal static class Vehicle_DamageLocation_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions
            .MethodReplacer(
                AccessTools.Method(typeof(Vehicle), nameof(applyStructureStatDamage)),
                AccessTools.Method(typeof(Vehicle_DamageLocation_Patch), nameof(applyStructureStatDamage))
            );
    }

    internal static void applyStructureStatDamage(
        Vehicle vehicle,
        VehicleChassisLocations location,
        float damage,
        WeaponHitInfo hitInfo
        )
    {
        try
        {
            if (!ComponentExplosionsFeature.IsInternalExplosion)
            {
                return;
            }

            var properties = ComponentExplosionsFeature.GetExplosionProtection<ExplosionProtectionStructureCustom>(vehicle, (int)location);
            if (properties?.MaximumDamage == null)
            {
                return;
            }

            var directDamage = Mathf.Min(damage, properties.MaximumDamage.Value);
            var backDamage = damage - directDamage;
            Log.Main.Debug?.Log($"reducing structure damage from {damage} to {directDamage} in {location}");
            damage = directDamage;

            if (backDamage <= 0)
            {
                return;
            }

            vehicle.PublishFloatieMessage("EXPLOSION REDIRECTED");

            var armorLocation = VehicleChassisLocations.Rear;
            var armor = vehicle.GetCurrentArmor(armorLocation);
            if (armor <= 0)
            {
                return;
            }

            var armorDamage = Mathf.Min(backDamage, armor);
            Log.Main.Debug?.Log($"added blowout armor damage {armorDamage} to {armorLocation}");

            vehicle.applyArmorStatDamage(armorLocation, armorDamage, hitInfo);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
        finally
        {
            vehicle.applyStructureStatDamage(location, damage, hitInfo);
        }
    }
}
