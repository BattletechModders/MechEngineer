using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer.Features.ComponentExplosions.Patches
{
    [HarmonyPatch(typeof(Mech), "DamageLocation")]
    internal static class Mech_DamageLocation_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(Mech), nameof(Mech.ApplyStructureStatDamage)),
                    AccessTools.Method(typeof(Mech_DamageLocation_Patch), nameof(ApplyStructureStatDamage))
                );
        }

        public static bool Prefix(ref bool __result)
        {
            if (!ComponentExplosionsFeature.IsInternalExplosionContained)
            {
                return true;
            }
            
            __result = false;
            Control.Logger.Debug?.Log("prevented explosion pass through");
            return false;
        }
        
        internal static void ApplyStructureStatDamage(
            this Mech mech,
            ChassisLocations location,
            float damage,
            WeaponHitInfo hitInfo
            )
        {
            try
            {
                if (damage <= 0)
                {
                    return; // ignore 0 damage calls
                }
                
                if (!ComponentExplosionsFeature.IsInternalExplosion)
                {
                    return;
                }

                var properties = ComponentExplosionsFeature.Shared.GetCASEProperties(mech, (int) location);
                if (properties == null)
                {
                    return;
                }

                ComponentExplosionsFeature.IsInternalExplosionContained = true;
                Control.Logger.Debug?.Log($"prevent explosion pass through from {Mech.GetAbbreviatedChassisLocation(location)}");
                
                if (properties.MaximumDamage == null)
                {
                    mech.PublishFloatieMessage("EXPLOSION CONTAINED");
                    return;
                }
                mech.PublishFloatieMessage("EXPLOSION REDIRECTED");
                
                var directDamage = Mathf.Min(damage, properties.MaximumDamage.Value);
                damage = directDamage; // update damage applied in finally
                
                if ((location & ChassisLocations.Torso) == 0)
                {
                    return;
                }
                
                var backDamage = damage - directDamage;
                Control.Logger.Debug?.Log($"reducing structure damage from {damage} to {directDamage} in {Mech.GetAbbreviatedChassisLocation(location)}");
                
                if (backDamage <= 0)
                {
                    return;
                }

                ArmorLocation armorLocation;
                switch (location)
                {
                    case ChassisLocations.LeftTorso:
                        armorLocation = ArmorLocation.LeftTorsoRear;
                        break;
                    case ChassisLocations.RightTorso:
                        armorLocation = ArmorLocation.RightTorsoRear;
                        break;
                    default:
                        armorLocation = ArmorLocation.CenterTorsoRear;
                        break;
                }

                var armor = mech.GetCurrentArmor(armorLocation);
                if (armor <= 0)
                {
                    return;
                }

                var armorDamage = Mathf.Min(backDamage, armor);
                Control.Logger.Debug?.Log($"added blowout armor damage {armorDamage} to {Mech.GetLongArmorLocation(armorLocation)}");

                mech.ApplyArmorStatDamage(armorLocation, armorDamage, hitInfo);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
            finally
            {
                mech.ApplyStructureStatDamage(location, damage, hitInfo);
            }
        }
    }
}
