using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "DamageLocation")]
    internal static class Mech_DamageLocation_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(Mech), nameof(Mech.GetCurrentArmor)),
                    AccessTools.Method(typeof(Mech_DamageLocation_Patch), nameof(OverrideGetCurrentArmor))
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(Mech), nameof(Mech.ApplyStructureStatDamage)),
                    AccessTools.Method(typeof(Mech_DamageLocation_Patch), nameof(OverrideApplyStructureStatDamage))
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(MechStructureRules), nameof(MechStructureRules.GetPassthroughLocation)),
                    AccessTools.Method(typeof(Mech_DamageLocation_Patch), nameof(OverrideGetPassthroughLocation))
                );
        }

        internal static bool IsInternalExplosion;

        internal static float OverrideGetCurrentArmor(
            this Mech mech,
            ArmorLocation location
            )
        {
            try
            {
                if (IsInternalExplosion)
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return mech.GetCurrentArmor(location);
        }

        internal static void OverrideApplyStructureStatDamage(
            this Mech mech,
            ChassisLocations location,
            float damage,
            WeaponHitInfo hitInfo
            )
        {
            try
            {
                if (IsInternalExplosion)
                {
                    var properties = ComponentExplosionHandler.Shared.GetCASEProperties(mech, (int) location);
                    if (properties?.MaximumDamage != null)
                    {
                        var directDamage = Mathf.Min(damage, properties.MaximumDamage.Value);
                        var backDamage = damage - directDamage;
                        //Control.mod.Logger.LogDebug($"reducing structure damage from {damage} to {directDamage} in {Mech.GetAbbreviatedChassisLocation(location)}");
                        damage = directDamage;

                        if (backDamage > 0)
                        {
                            mech.PublishFloatieMessage("EXPLOSION REDIRECTED");

                            if ((location & ChassisLocations.Torso) > 0)
                            {
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
                                if (armor > 0)
                                {
                                    var armorDamage = Mathf.Min(backDamage, armor);
                                    //Control.mod.Logger.LogDebug($"added blowout armor damage {armorDamage} to {Mech.GetLongArmorLocation(armorLocation)}");

                                    mech.ApplyArmorStatDamage(armorLocation, armorDamage, hitInfo);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            mech.ApplyStructureStatDamage(location, damage, hitInfo);
        }

        internal static ArmorLocation OverrideGetPassthroughLocation(
            ArmorLocation location,
            AttackDirection attackDirection
            )
        {
            try
            {
                if (IsInternalExplosion && currentMech != null)
                {
                    var chassisLocation = MechStructureRules.GetChassisLocationFromArmorLocation(location);
                    var properties = ComponentExplosionHandler.Shared.GetCASEProperties(currentMech, (int) chassisLocation);
                    if (properties != null)
                    {
                        currentMech.PublishFloatieMessage("EXPLOSION CONTAINED");

                        //Control.mod.Logger.LogDebug($"prevented explosion pass through from {Mech.GetAbbreviatedChassisLocation(chassisLocation)}");

                        return ArmorLocation.None; // CASE redirects damage, so lets redirect it to none
                    }
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return MechStructureRules.GetPassthroughLocation(location, attackDirection);
        }

        private static Mech currentMech;

        internal static void Prefix(Mech __instance)
        {
            currentMech = __instance;
        }

        internal static void Postfix(Mech __instance, int originalHitLoc, WeaponHitInfo hitInfo, ArmorLocation aLoc, Weapon weapon, float totalDamage, int hitIndex, AttackImpactQuality impactQuality, bool __result)
        {
            currentMech = null;
        }
    }
}
