using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.AutoFix.Patches
{
    [HarmonyPatch(typeof(WeaponDef), "FromJSON")]
    public static class WeaponDef_FromJSON_Patch
    {
        public static void Postfix(WeaponDef __instance)
        {
            try
            {
                var changes = Control.settings.AutoFixWeaponDefSlotsChanges;
                if (changes == null)
                {
                    return;
                }
                
                foreach (var change in changes.Where(x => x.Type == __instance.WeaponSubType).Select(x => x.Change))
                {
                    var newValue = change.Change(__instance.InventorySize);
                    if (!newValue.HasValue)
                    {
                        return;
                    }
                    Traverse.Create(__instance).Property("InventorySize").SetValue(newValue);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
