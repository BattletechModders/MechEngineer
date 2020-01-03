using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

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
            // normalize data, remove all duplicates in a location
            for (var di = 0; di < def.HardpointData.Length; di++)
            {
                var used = new HashSet<string>();
                for (var wsi = 0; wsi < def.HardpointData[di].weapons.Length; wsi++)
                {
                    var weaponSetList = def.HardpointData[di].weapons[wsi].ToList();
                    foreach (var weapon in def.HardpointData[di].weapons[wsi])
                    {
                        if (used.Contains(weapon))
                        {
                            Control.mod.Logger.LogWarning($"Removing duplicate {weapon} entry from {def.ID}");
                            weaponSetList.Remove(weapon);
                        }
                        else
                        {
                            used.Add(weapon);
                        }
                    }
                    def.HardpointData[di].weapons[wsi] = weaponSetList.ToArray();
                }
                def.HardpointData[di].weapons = def.HardpointData[di].weapons.Where(x => x.Length > 0).ToArray();
            }
        }
    }
}