using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;
using MechEngineer.Features.HardpointFix.utils;

namespace MechEngineer.Features.HardpointFix.sorting.Patches
{
    [HarmonyPatch(typeof(HardpointDataDef), nameof(HardpointDataDef.FromJSON))]
    public static class HardpointDataDef_FromJSON_Patch
    {
        public static void Postfix(HardpointDataDef __instance)
        {
            try
            {
                HardpointDataDef_FromJSON(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static void HardpointDataDef_FromJSON(HardpointDataDef def)
        {
            if (!HardpointFixFeature.Shared.Settings.AutoFixHardpointDataDefs)
            {
                return;
            }

            // chrPrfWeap_battlemaster_leftarm_ac20_bh1 -> chrPrfWeap_battlemaster_leftarm_ac20_1
            string Unique(string prefab)
            {
                return prefab.Substring(0, prefab.Length - 3) + WeaponComponentPrefabCalculator.GroupNumber(prefab);
            }

            string SelectSingle(IEnumerable<string> enumerable)
            {
                var list = enumerable.ToList();
                var chosen = list[0];
                if (list.Count > 1)
                {
                    var duplicates = string.Join(",", list.OrderBy(x => x).Distinct());
                    Control.mod.Logger.LogWarning($"Removing duplicate hardpoint entries [{duplicates}], kept a single {chosen}");
                }
                return chosen;
            }

            // normalize data, remove all duplicates in each location
            for (var i = 0; i < def.HardpointData.Length; i++)
            {
                def.HardpointData[i].weapons = def.HardpointData[i].weapons
                    .SelectMany(x => x)
                    .OrderBy(x => x)
                    .GroupBy(x => Unique(x))
                    .Select(x => SelectSingle(x))
                    .GroupBy(x => WeaponComponentPrefabCalculator.GroupNumber(x))
                    .OrderBy(x => x.Key)
                    .Select(x => x.ToArray())
                    .ToArray();
            }
        }
    }
}