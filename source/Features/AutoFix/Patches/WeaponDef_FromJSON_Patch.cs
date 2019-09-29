using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.AutoFix.Patches
{
    [HarmonyPatch(typeof(WeaponDef), nameof(Weapon.FromJSON))]
    public static class WeaponDef_FromJSON_Patch
    {
        public static void Postfix(WeaponDef __instance)
        {
            try
            {
                var changes = AutoFixerFeature.settings.AutoFixWeaponDefSlotsChanges;
                if (changes == null)
                {
                    return;
                }

                if (AutoFixUtils.IsIgnoredByTags(__instance.ComponentTags, AutoFixerFeature.settings.WeaponDefTagsSkip))
                {
                    return;
                }
                
                foreach (var change in changes.Where(x => x.Type == __instance.WeaponSubType))
                {
                    if (change.SlotChange != null)
                    {
                        var newValue = change.SlotChange.Change(__instance.InventorySize);
                        if (!newValue.HasValue)
                        {
                            return;
                        }
                        Traverse.Create(__instance).Property("InventorySize").SetValue(newValue);
                    }

                    if (change.TonnageChange != null)
                    {
                        var newValue = change.TonnageChange.Change(__instance.Tonnage);
                        if (!newValue.HasValue)
                        {
                            return;
                        }
                        Traverse.Create(__instance).Property("Tonnage").SetValue(newValue);
                    }
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
