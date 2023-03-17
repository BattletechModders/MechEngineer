using BattleTech;
using BattleTech.UI.Tooltips;

namespace MechEngineer.Features.OverrideDescriptions.Patches;

[HarmonyPatch(typeof(TooltipPrefab_Weapon), nameof(TooltipPrefab_Weapon.SetData))]
public static class TooltipPrefab_WeaponSetData_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(TooltipPrefab_Weapon __instance, object data)
    {
        if (data is MechComponentDef def)
        {
            OverrideDescriptionsFeature.Shared.AdjustTooltipWeapon(__instance, def);
        }
    }
}
