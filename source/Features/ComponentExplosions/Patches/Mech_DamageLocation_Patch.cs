using System;
using BattleTech;
using Harmony;
using MechEngineer.Helper;
using UnityEngine;

namespace MechEngineer.Features.ComponentExplosions.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.DamageLocation))]
internal static class Mech_DamageLocation_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(
        Mech __instance,
        WeaponHitInfo hitInfo,
        ArmorLocation aLoc,
        ref float directStructureDamage,
        ref bool __result)
    {
        try
        {
            if (ComponentExplosionsFeature.IsInternalExplosionContained)
            {
                __result = false;
                Control.Logger.Warning?.Log("prevented explosion pass through (you should never see this message)");
                return false;
            }

            if (ComponentExplosionsFeature.IsInternalExplosion)
            {
                var location = MechStructureRules.GetChassisLocationFromArmorLocation(aLoc);
                UpdateStructureDamage(__instance, location, hitInfo, ref directStructureDamage);
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }

        return true;
    }

    internal static void UpdateStructureDamage(
        this Mech mech,
        ChassisLocations location,
        WeaponHitInfo hitInfo,
        ref float damage
        )
    {
        if (damage <= 0)
        {
            return; // ignore 0 damage calls
        }

        var properties = ComponentExplosionsFeature.Shared.GetCASEProperties(mech, (int)location);
        if (properties == null)
        {
            return;
        }

        ComponentExplosionsFeature.IsInternalExplosionContained = true;
        Control.Logger.Debug?.Log($"prevent explosion pass through from {Mech.GetAbbreviatedChassisLocation(location)}");

        var maxStructureDamage = mech.GetCurrentStructure(location);
        Control.Logger.Debug?.Log($"damage={damage} maxStructureDamage={maxStructureDamage}");

        if (properties.MaximumDamage == null)
        {
            damage = Mathf.Min(maxStructureDamage, damage);
            mech.PublishFloatieMessage("EXPLOSION CONTAINED");
            return;
        }

        var newInternalDamage = Mathf.Min(damage, properties.MaximumDamage.Value);
        var backDamage = damage - newInternalDamage;
        Control.Logger.Debug?.Log($"reducing structure damage from {damage} to {newInternalDamage}");

        damage = Mathf.Min(maxStructureDamage, newInternalDamage);
        mech.PublishFloatieMessage("EXPLOSION REDIRECTED");

        if (backDamage <= 0)
        {
            return;
        }

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
}