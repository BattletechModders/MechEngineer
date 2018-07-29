using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(WeaponDef), "FromJSON")]
    public static class WeaponDef_FromJSON_Patch
    {
        private static Dictionary<WeaponSubType, ValueChange<int>> lookupDictionary;

        public static void Postfix(WeaponDef __instance)
        {
            try
            {
                var changes = Control.settings.AutoFixWeaponDefSlotsChanges;
                if (changes == null)
                {
                    return;
                }

                if (lookupDictionary == null)
                {
                    lookupDictionary = changes.ToDictionary(x => x.Type, x => x.Change);
                }

                if (!lookupDictionary.TryGetValue(__instance.WeaponSubType, out var change))
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
