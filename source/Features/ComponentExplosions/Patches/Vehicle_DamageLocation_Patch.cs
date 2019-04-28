using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer.Features.ComponentExplosions.Patches
{
    [HarmonyPatch(typeof(Vehicle), "DamageLocation")]
    internal static class Vehicle_DamageLocation_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(Vehicle), nameof(Vehicle.GetCurrentArmor)),
                    AccessTools.Method(typeof(Vehicle_DamageLocation_Patch), nameof(GetCurrentArmor))
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(Vehicle), nameof(applyStructureStatDamage)),
                    AccessTools.Method(typeof(Vehicle_DamageLocation_Patch), nameof(applyStructureStatDamage))
                );
        }

        internal static float GetCurrentArmor(
            Vehicle vehicle,
            VehicleChassisLocations location
            )
        {
            try
            {
                if (ComponentExplosionsFeature.IsInternalExplosion)
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return vehicle.GetCurrentArmor(location);
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

                var properties = ComponentExplosionsFeature.Shared.GetCASEProperties(vehicle, (int) location);
                if (properties?.MaximumDamage == null)
                {
                    return;
                }

                var directDamage = Mathf.Min(damage, properties.MaximumDamage.Value);
                var backDamage = damage - directDamage;
                //Control.mod.Logger.LogDebug($"reducing structure damage from {damage} to {directDamage} in {Mech.GetAbbreviatedChassisLocation(location)}");
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
                //Control.mod.Logger.LogDebug($"added blowout armor damage {armorDamage} to {Mech.GetLongArmorLocation(armorLocation)}");

                vehicle.applyArmorStatDamage(armorLocation, armorDamage, hitInfo);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            finally
            {
                vehicle.applyStructureStatDamage(location, damage, hitInfo);
            }
        }
    }
}
