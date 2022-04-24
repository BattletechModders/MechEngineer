using System;
using BattleTech;
using BattleTech.UI.Tooltips;
using Harmony;

namespace MechEngineer.Features.OverrideDescriptions.Patches;

[HarmonyPatch(typeof(TooltipPrefab_Weapon), nameof(TooltipPrefab_Weapon.SetData))]
public static class TooltipPrefab_WeaponSetData_Patch
{
    public static void Postfix(TooltipPrefab_Weapon __instance, object data)
    {
        try
        {
            if (data is MechComponentDef def)
            {
                OverrideDescriptionsFeature.Shared.AdjustTooltipWeapon(__instance, def);
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}