using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using MechEngineer.Helper;
using UnityEngine;

namespace MechEngineer.Features.ComponentExplosions.Patches;

[HarmonyPatch(typeof(Vehicle), nameof(Vehicle.DamageLocation))]
internal static class Vehicle_DamageLocation_Patch
{
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

            var properties = ComponentExplosionsFeature.Shared.GetCASEProperties(vehicle, (int)location);
            if (properties?.MaximumDamage == null)
            {
                return;
            }

            var directDamage = Mathf.Min(damage, properties.MaximumDamage.Value);
            var backDamage = damage - directDamage;
            Control.Logger.Debug?.Log($"reducing structure damage from {damage} to {directDamage} in {location}");
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
            Control.Logger.Debug?.Log($"added blowout armor damage {armorDamage} to {armorLocation}");

            vehicle.applyArmorStatDamage(armorLocation, armorDamage, hitInfo);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        finally
        {
            vehicle.applyStructureStatDamage(location, damage, hitInfo);
        }
    }
}