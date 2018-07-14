using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(WeaponDef), "FromJSON")]
    public static class WeaponDef_FromJSON_Patch
    {
        public static void Postfix(WeaponDef __instance)
        {
            try
            {
                if (!Control.settings.AutoFixWeaponDefSlots)
                {
                    return;
                }

                if (!Control.settings.AutoFixWeaponDefSlotsChanges.TryGetValue(__instance.WeaponSubType.ToString(), out var change))
                {
                    return;
                }

                var newValue = change.Change(__instance.InventorySize);
                if (!newValue.HasValue)
                {
                    return;
                }

                Traverse.Create(__instance).Property("InventorySize").SetValue(newValue);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
