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
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(MechStructureRules), nameof(MechStructureRules.GetPassthroughLocation)),
                    AccessTools.Method(typeof(Mech_DamageLocation_Patch), nameof(GetPassthroughLocation))
                );
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
                if (!ComponentExplosionsFeature.IsInternalExplosion)
                {
                    return;
                }

                var properties = ComponentExplosionsFeature.Shared.GetCASEProperties(mech, (int) location);
                if (properties?.MaximumDamage == null)
                {
                    return;
                }

                var directDamage = Mathf.Min(damage, properties.MaximumDamage.Value);
                var backDamage = damage - directDamage;
                Control.Logger.Debug?.Log($"reducing structure damage from {damage} to {directDamage} in {Mech.GetAbbreviatedChassisLocation(location)}");
                damage = directDamage;

                if (backDamage <= 0)
                {
                    return;
                }

                mech.PublishFloatieMessage("EXPLOSION REDIRECTED");

                if ((location & ChassisLocations.Torso) == 0)
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

        internal static ArmorLocation GetPassthroughLocation(
            ArmorLocation location,
            AttackDirection attackDirection
            )
        {
            try
            {
                if (ComponentExplosionsFeature.IsInternalExplosion && currentMech != null)
                {
                    var chassisLocation = MechStructureRules.GetChassisLocationFromArmorLocation(location);
                    var properties = ComponentExplosionsFeature.Shared.GetCASEProperties(currentMech, (int) chassisLocation);
                    if (properties != null)
                    {
                        currentMech.PublishFloatieMessage("EXPLOSION CONTAINED");

                        Control.Logger.Debug?.Log($"prevented explosion pass through from {Mech.GetAbbreviatedChassisLocation(chassisLocation)}");

                        return ArmorLocation.None; // CASE redirects damage, so lets redirect it to none
                    }
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }

            return MechStructureRules.GetPassthroughLocation(location, attackDirection);
        }

        private static Mech currentMech;

        internal static void Prefix(Mech __instance)
        {
            currentMech = __instance;
        }

        internal static void Postfix(Mech __instance)
        {
            currentMech = null;
        }
    }
}
